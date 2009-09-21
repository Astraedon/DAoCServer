/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 * 
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace SQLite.Designer
{
  using System;
  using Microsoft.VisualStudio.Data;
  using System.Windows.Forms.Design;
  using Microsoft.VisualStudio.Shell.Interop;
  using Microsoft.VisualStudio;
  using Microsoft.VisualStudio.OLE.Interop;
  using System.Data.Common;
  using SQLite.Designer.Editors;

  enum cmdid
  {
    CreateTable = 0x3520,
    CreateView = 0x3521,
    Alter = 0x3003,
    Delete = 17,
    Vacuum = 262,
    Rekey = 263,
  }

  internal sealed class SQLiteCommandHandler : DataViewCommandHandler
  {
    internal static readonly Guid guidDataCmdSet = new Guid("501822E1-B5AF-11d0-B4DC-00A0C91506EF");
    internal static readonly Guid guidSQLiteCmdSet = new Guid("814658EE-A28E-4b97-BC33-4B1BC81EBECB");
    internal static readonly Guid guidIFCmdId = new Guid("{74d21311-2aee-11d1-8bfb-00a0c90f26f7}");
    internal static readonly Guid guidDavinci = new Guid("{732abe75-cd80-11d0-a2db-00aa00a3efff}");
    internal static readonly Guid guidDavinciGrp = new Guid("{732abe74-cd80-11d0-a2db-00aa00a3efff}");

    public SQLiteCommandHandler()
    {
    }

    public override OleCommandStatus GetCommandStatus(int[] itemIds, OleCommand command, OleCommandTextType textType, OleCommandStatus status)
    {
      if (command.GroupGuid == guidSQLiteCmdSet)
      {
        switch ((cmdid)command.CommandId)
        {
          case cmdid.CreateTable:
          case cmdid.Vacuum:
          case cmdid.Rekey:
            status.Supported = true;
            status.Visible = true;
            status.Enabled = true;
            return status;
        }
      }
      else if (command.GroupGuid == VSConstants.GUID_VSStandardCommandSet97)
      {
        switch ((VSConstants.VSStd97CmdID)command.CommandId)
        {
          case VSConstants.VSStd97CmdID.Delete:
            status.Supported = true;
            status.Visible = true;
            status.Enabled = (SystemTableSelected == false && SystemIndexSelected == false);
            return status;
        }
      }
      else if (command.GroupGuid == guidDataCmdSet)
      {
        switch ((cmdid)command.CommandId)
        {
          case cmdid.Alter:
            status.Supported = true;
            status.Visible = true;
            status.Enabled = (SystemTableSelected == false && SystemIndexSelected == false);
            return status;
          //case cmdid.CreateTable:
          //case cmdid.CreateView:
          //  status.Supported = true;
          //  status.Visible = true;
          //  status.Enabled = true;
          //  return status;
        }
      }
      base.GetCommandStatus(itemIds, command, textType, status);

      return status;
    }

    private bool SystemTableSelected
    {
      get
      {
        int[] items = DataViewHierarchyAccessor.GetSelectedItems();
        int n;
        object[] parts;

        for (n = 0; n < items.Length; n++)
        {
          parts = DataViewHierarchyAccessor.GetObjectIdentifier(items[n]);
          if (parts == null) return true;

          if (parts[2].ToString().StartsWith("sqlite_", StringComparison.InvariantCultureIgnoreCase))
            return true;
        }
        return false;
      }
    }

    private bool SystemIndexSelected
    {
      get
      {
        int[] items = DataViewHierarchyAccessor.GetSelectedItems();
        int n;
        object[] parts;

        for (n = 0; n < items.Length; n++)
        {
          parts = DataViewHierarchyAccessor.GetObjectIdentifier(items[n]);
          if (parts == null) return true;

          if (parts[2].ToString().StartsWith("sqlite_", StringComparison.InvariantCultureIgnoreCase))
            return true;

          if (parts.Length > 3)
          {
            if (parts[3].ToString().StartsWith("sqlite_autoindex_", StringComparison.InvariantCultureIgnoreCase)
              || parts[3].ToString().StartsWith("sqlite_master_PK_", StringComparison.InvariantCultureIgnoreCase))
              return true;
          }
        }
        return false;
      }
    }

    /// <summary>
    /// This method executes a specified command, potentially based
    /// on parameters passed in from the data view support XML.
    /// </summary>
    public override object ExecuteCommand(int itemId, OleCommand command, OleCommandExecutionOption executionOption, object arguments)
    {
      object returnValue = null;
      object[] args = arguments as object[];

      if (command.GroupGuid == guidSQLiteCmdSet)
      {
        switch ((cmdid)command.CommandId)
        {
          case cmdid.Vacuum:
            Vacuum();
            break;
          case cmdid.Rekey:
            ChangePassword(itemId);
            break;
          default:
            returnValue = base.ExecuteCommand(itemId, command, executionOption, arguments);
            break;
        }
      }
      else if (command.GroupGuid == VSConstants.GUID_VSStandardCommandSet97)
      {
        switch ((VSConstants.VSStd97CmdID)command.CommandId)
        {
          case VSConstants.VSStd97CmdID.Delete:
            switch ((string)args[0])
            {
              case "Table":
                DropSelectedTables();
                break;
              case "Index":
                DropSelectedIndexes();
                break;
              case "View":
                DropSelectedViews();
                break;
            }
            break;
        }
      }
      else if (command.GroupGuid == guidDataCmdSet)
      {
        switch ((cmdid)command.CommandId)
        {
          case cmdid.CreateTable:
            CreateTable(itemId);
            break;
          case cmdid.CreateView:
            CreateView(itemId);
            break;
          case cmdid.Alter:
            switch ((string)args[0])
            {
              case "Table":
                break;
              case "Index":
                break;
              case "View":
                break;
            }
            break;
        }
      }
      else
      {
        returnValue = base.ExecuteCommand(itemId, command, executionOption, arguments);
      }
      return returnValue;
    }

    private void CreateTable(int itemId)
    {
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider = DataViewHierarchyAccessor.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
      IVsUIShell shell = DataViewHierarchyAccessor.ServiceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;
      IVsUIHierarchy hier = DataViewHierarchyAccessor.Hierarchy;
      IVsWindowFrame frame;

      if (shell != null)
      {
        TableDesignerDoc form = new TableDesignerDoc(DataViewHierarchyAccessor.Connection, null);
        IntPtr formptr = System.Runtime.InteropServices.Marshal.GetIUnknownForObject(form);
        Guid empty = Guid.Empty;
        FakeHierarchy fake = new FakeHierarchy(form, hier);

        int code = shell.CreateDocumentWindow(
          0, // (uint)(__VSCREATEDOCWIN.CDW_fCreateNewWindow | __VSCREATEDOCWIN.CDW_RDTFLAGS_MASK) | (uint)(_VSRDTFLAGS.RDT_CanBuildFromMemory | _VSRDTFLAGS.RDT_NonCreatable | _VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_DontAddToMRU),
          form.Name, fake, (uint)itemId, formptr, formptr, ref empty, null, ref empty, provider, "SQLite:", form.Name, null, out frame);

        if (frame != null)
        {
          object ret;
          int prop = (int)__VSFPROPID.VSFPROPID_Caption;
          
          code = frame.GetProperty(prop, out ret);

          code = frame.Show();
        }
      }
      // TODO: Implement this command
    }

    private void CreateView(int itemId)
    {
      // TODO: Implement this command
    }

    private void DropSelectedTables()
    {
      int[] items = DataViewHierarchyAccessor.GetSelectedItems();
      int n;
      object[] parts;

      for (n = 0; n < items.Length; n++)
      {
        parts = DataViewHierarchyAccessor.GetObjectIdentifier(items[n]);
        if (parts == null) continue;

        if (System.Windows.Forms.MessageBox.Show(String.Format("Drop table {0} ({1}), are you sure?", parts[2], parts[0]), "Confirm delete", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
          string sql = String.Format("DROP TABLE [{0}].[{1}]", parts[0], parts[2]);

          DataViewHierarchyAccessor.Connection.Command.ExecuteWithoutResults(sql, (int)System.Data.CommandType.Text, null, 0);
          DataViewHierarchyAccessor.DropObjectNode(items[n]);
        }
        else throw new OperationCanceledException();
      }
    }

    private void DropSelectedViews()
    {
      int[] items = DataViewHierarchyAccessor.GetSelectedItems();
      int n;
      object[] parts;

      for (n = 0; n < items.Length; n++)
      {
        parts = DataViewHierarchyAccessor.GetObjectIdentifier(items[n]);
        if (parts == null) continue;

        if (System.Windows.Forms.MessageBox.Show(String.Format("Drop view {0} ({1}), are you sure?", parts[2], parts[0]), "Confirm delete", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
          string sql = String.Format("DROP VIEW [{0}].[{1}]", parts[0], parts[2]);

          DataViewHierarchyAccessor.Connection.Command.ExecuteWithoutResults(sql, (int)System.Data.CommandType.Text, null, 0);
          DataViewHierarchyAccessor.DropObjectNode(items[n]);
        }
        else throw new OperationCanceledException();
      }
    }

    private void DropSelectedIndexes()
    {
      int[] items = DataViewHierarchyAccessor.GetSelectedItems();
      int n;
      object[] parts;

      for (n = 0; n < items.Length; n++)
      {
        parts = DataViewHierarchyAccessor.GetObjectIdentifier(items[n]);
        if (parts == null) continue;

        if (System.Windows.Forms.MessageBox.Show(String.Format("Drop index {0} ({1}), are you sure?", parts[3], parts[0]), "Confirm delete", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        {
          string sql = String.Format("DROP INDEX [{0}].[{1}]", parts[0], parts[3]);

          DataViewHierarchyAccessor.Connection.Command.ExecuteWithoutResults(sql, (int)System.Data.CommandType.Text, null, 0);
          DataViewHierarchyAccessor.DropObjectNode(items[n]);
        }
        else throw new OperationCanceledException();
      }
    }

    private void Vacuum()
    {
      DataViewHierarchyAccessor.Connection.Command.ExecuteWithoutResults("VACUUM", (int)System.Data.CommandType.Text, null, 0);
    }

    private void ChangePassword(int itemId)
    {
      DataConnection dataConn = DataViewHierarchyAccessor.Connection;
      DbConnection cnn = DataViewHierarchyAccessor.Connection.ConnectionSupport.ProviderObject as DbConnection;
      if (cnn == null) return;

      SQLiteConnectionProperties props = new SQLiteConnectionProperties(cnn.ConnectionString);

      using (ChangePasswordDialog dlg = new ChangePasswordDialog(props))
      {
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          if (String.IsNullOrEmpty(dlg.Password))
            props.Remove("Password");
          else
            props["Password"] = dlg.Password;

          System.Reflection.MethodInfo method = cnn.GetType().GetMethod("ChangePassword", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, new Type[] { typeof(string) }, null);

          if (method != null)
          {
            method.Invoke(cnn, new object[] { dlg.Password });

            dataConn.Close();
            dataConn.DisplayConnectionString = props.ToDisplayString();
            dataConn.Open();

            Refresh(itemId);
          }
        }
      }
    }

    private void Refresh(int itemId)
    {
      IVsUIHierarchy hier = DataViewHierarchyAccessor.Hierarchy as IVsUIHierarchy;

      Guid g = VSConstants.GUID_VSStandardCommandSet97;
      hier.ExecCommand((uint)itemId, ref g, (uint)0xbd, (uint)OleCommandExecutionOption.DoDefault, IntPtr.Zero, IntPtr.Zero);
    }
  }
}