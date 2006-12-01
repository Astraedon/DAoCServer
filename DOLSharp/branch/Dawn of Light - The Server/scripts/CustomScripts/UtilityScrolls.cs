using System;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS;
using DOL.GS.Effects;
using DOL.GS.GameEvents;

namespace DOL.GS.GameEvents
{
	public class UtilityScrollsEvent
	{
		/*
		 * TODO: add loot generator for dropping scrolls
		 * add loot generator for dropping seals
		 * change merchant types in df to seal merchant
		 * hasteners
		 * vault keepers
		 * teleporter
		 * recharger
		 * smith
		 */
		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{ }

		#region Spells
		#region Trainer
		protected static Spell m_trainerSpell;
		public static Spell TrainerSpell
		{
			get
			{
				if (m_trainerSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.ClientEffect = 0;
					spell.Duration = 60;
					spell.Name = "Trainer Spell";
					spell.Range = 0;
					spell.SpellID = 64000;
					spell.Target = "Self";
					spell.Type = "UtilityNPC";
					spell.Value = TrainerTemplate.TemplateId;
					m_trainerSpell = new Spell(spell, 1);
					SkillBase.GetSpellList(GlobalSpellsLines.Item_Effects).Add(m_trainerSpell);
				}
				return m_trainerSpell;
			}
		}
		#endregion
		#region Merchant
		protected static Spell m_merchantSpell;
		public static Spell MerchantSpell
		{
			get
			{
				if (m_merchantSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.ClientEffect = 0;
					spell.Duration = 60;
					spell.Name = "Merchant Spell";
					spell.Range = 0;
					spell.SpellID = 64001;
					spell.Target = "Self";
					spell.Type = "UtilityNPC";
					spell.Value = MerchantTemplate.TemplateId;
					m_merchantSpell = new Spell(spell, 1);
					SkillBase.GetSpellList(GlobalSpellsLines.Item_Effects).Add(m_merchantSpell);
				}
				return m_merchantSpell;
			}
		}
		#endregion
		#region Healer
		protected static Spell m_healerSpell;
		public static Spell HealerSpell
		{
			get
			{
				if (m_healerSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.ClientEffect = 0;
					spell.Duration = 60;
					spell.Name = "Healer Spell";
					spell.Range = 0;
					spell.SpellID = 64002;
					spell.Target = "Self";
					spell.Type = "UtilityNPC";
					spell.Value = HealerTemplate.TemplateId;
					m_healerSpell = new Spell(spell, 1);
					SkillBase.GetSpellList(GlobalSpellsLines.Item_Effects).Add(m_healerSpell);
				}
				return m_healerSpell;
			}
		}
		#endregion
		#endregion

		#region Items
		#region Trainer
		protected static ItemTemplate m_trainerScroll;
		public static ItemTemplate TrainerScroll
		{
			get
			{
				if (m_trainerScroll == null)
				{
					m_trainerScroll = new ItemTemplate();
					m_trainerScroll.CanDropAsLoot = false;
					m_trainerScroll.Charges = 1;
					m_trainerScroll.Id_nb = "trainer_scroll";
					m_trainerScroll.IsDropable = true;
					m_trainerScroll.IsPickable = true;
					m_trainerScroll.IsTradable = true;
					m_trainerScroll.Item_Type = 41;
					m_trainerScroll.Level = 1;
					m_trainerScroll.MaxCharges = 1;
					m_trainerScroll.Model = 499;
					m_trainerScroll.Name = "Trainer Scroll";
					m_trainerScroll.Object_Type = (int)eObjectType.Magical;
					m_trainerScroll.Realm = 0;
					m_trainerScroll.SpellID = TrainerSpell.ID;
				}
				return m_trainerScroll;
			}
		}
		#endregion
		#region Merchant
		protected static ItemTemplate m_merchantScroll;
		public static ItemTemplate MerchantScroll
		{
			get
			{
				if (m_merchantScroll == null)
				{
					m_merchantScroll = new ItemTemplate();
					m_merchantScroll.CanDropAsLoot = false;
					m_merchantScroll.Charges = 1;
					m_merchantScroll.Id_nb = "merchant_scroll";
					m_merchantScroll.IsDropable = true;
					m_merchantScroll.IsPickable = true;
					m_merchantScroll.IsTradable = true;
					m_merchantScroll.Item_Type = 41;
					m_merchantScroll.Level = 1;
					m_merchantScroll.MaxCharges = 1;
					m_merchantScroll.Model = 499;
					m_merchantScroll.Name = "Merchant Scroll";
					m_merchantScroll.Object_Type = (int)eObjectType.Magical;
					m_merchantScroll.Realm = 0;
					m_merchantScroll.SpellID = MerchantSpell.ID;
				}
				return m_merchantScroll;
			}
		}
		#endregion
		#region Healer
		protected static ItemTemplate m_healerScroll;
		public static ItemTemplate HealerScroll
		{
			get
			{
				if (m_healerScroll == null)
				{
					m_healerScroll = new ItemTemplate();
					m_healerScroll.CanDropAsLoot = false;
					m_healerScroll.Charges = 1;
					m_healerScroll.Id_nb = "healer_scroll";
					m_healerScroll.IsDropable = true;
					m_healerScroll.IsPickable = true;
					m_healerScroll.IsTradable = true;
					m_healerScroll.Item_Type = 41;
					m_healerScroll.Level = 1;
					m_healerScroll.MaxCharges = 1;
					m_healerScroll.Model = 499;
					m_healerScroll.Name = "Healer Scroll";
					m_healerScroll.Object_Type = (int)eObjectType.Magical;
					m_healerScroll.Realm = 0;
					m_healerScroll.SpellID = HealerSpell.ID;
				}
				return m_healerScroll;
			}
		}
		#endregion
		#endregion

		#region NPC Template
		#region Trainer
		protected static NpcTemplate m_trainerTemplate;
		public static NpcTemplate TrainerTemplate
		{
			get
			{
				if (m_trainerTemplate == null)
				{
					m_trainerTemplate = new NpcTemplate();
					m_trainerTemplate.Flags += (byte)GameNPC.eFlags.PEACE;
					m_trainerTemplate.Flags += (byte)GameNPC.eFlags.TRANSPARENT;
					m_trainerTemplate.GuildName = "Trainer";
					m_trainerTemplate.Name = "Summoned Trainer";
					m_trainerTemplate.ClassType = "DOL.GS.Trainer.GenericTrainer";
					m_trainerTemplate.Model = "50";
					m_trainerTemplate.TemplateId = 600;
					NpcTemplateMgr.AddTemplate(m_trainerTemplate);
				}
				return m_trainerTemplate;
			}
		}
		#endregion
		#region Merchant
		protected static NpcTemplate m_merchantTemplate;
		public static NpcTemplate MerchantTemplate
		{
			get
			{
				if (m_merchantTemplate == null)
				{
					m_merchantTemplate = new NpcTemplate();
					m_merchantTemplate.Flags += (byte)GameNPC.eFlags.PEACE;
					m_merchantTemplate.Flags += (byte)GameNPC.eFlags.TRANSPARENT;
					m_merchantTemplate.GuildName = "Merchant";
					m_merchantTemplate.Name = "Summoned Merchant";
					m_merchantTemplate.ClassType = "DOL.GS.GameMerchant";
					m_merchantTemplate.Model = "50";
					m_merchantTemplate.TemplateId = 601;
					NpcTemplateMgr.AddTemplate(m_merchantTemplate);
				}
				return m_merchantTemplate;
			}
		}
		#endregion
		#region Healer
		protected static NpcTemplate m_healerTemplate;
		public static NpcTemplate HealerTemplate
		{
			get
			{
				if (m_healerTemplate == null)
				{
					m_healerTemplate = new NpcTemplate();
					m_healerTemplate.Flags += (byte)GameNPC.eFlags.PEACE;
					m_healerTemplate.Flags += (byte)GameNPC.eFlags.TRANSPARENT;
					m_healerTemplate.GuildName = "Healer";
					m_healerTemplate.Name = "Summoned Healer";
					m_healerTemplate.ClassType = "DOL.GS.GameHealer";
					m_healerTemplate.Model = "50";
					m_healerTemplate.TemplateId = 602;
					NpcTemplateMgr.AddTemplate(m_healerTemplate);
				}
				return m_healerTemplate;
			}
		}
		#endregion
		#endregion
	}
}

namespace DOL.GS
{
	public class LootGeneratorUtilityScrolls : LootGeneratorBase
	{
		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			LootList list = base.GenerateLoot(mob, killer);
			return list;
		}
	}
}

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("UtilityNPC")]
	public class UtilityNPCSpellHandler : SpellHandler
	{
		public UtilityNPCSpellHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{ }

		protected GameNPC npc = null;

		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			base.ApplyEffectOnTarget(target, effectiveness);

			NpcTemplate template = NpcTemplateMgr.GetTemplate((int)m_spell.Value);

			if (template.ClassType == "")
				npc = new GameNPC();
			else
			{
				try
				{
					npc = (GameNPC)Assembly.GetAssembly(typeof(GameServer)).CreateInstance(template.ClassType, false);
				}
				catch (Exception e)
				{
					log.Error(e);
				}
				if (npc == null)
				{
					try
					{
						npc = (GameNPC)Assembly.GetExecutingAssembly().CreateInstance(template.ClassType, false);
					}
					catch (Exception e)
					{
						log.Error(e);
					}
				}
				if (npc == null)
				{
					MessageToCaster("There was an error creating an instance of " + template.ClassType + "!", DOL.GS.PacketHandler.eChatType.CT_System);
					return;
				}
				npc.LoadTemplate(template);
			}
			GameSpellEffect effect = CreateSpellEffect(npc, effectiveness);
			int x, y;
			m_caster.GetSpotFromHeading(64, out x, out y);
			npc.X = x;
			npc.Y = y;
			npc.Z = m_caster.Z;
			npc.CurrentRegion = m_caster.CurrentRegion;
			npc.Heading = (ushort)((m_caster.Heading + 2048) % 4096);
			npc.Realm = m_caster.Realm;
			npc.CurrentSpeed = 0;
			npc.Level = 1;
			npc.SetOwnBrain(new AI.Brain.BlankBrain());
			npc.AddToWorld();
			effect.Start(npc);
		}

		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			if (npc != null)
				npc.Delete();
			return base.OnEffectExpires(effect, noMessages);
		}

		public override bool IsOverwritable(GameSpellEffect compare)
		{
			return false;
		}
	}
}

namespace DOL.GS.Scripts
{
	[CmdAttribute("&scroll", //command to handle
		2, //minimum privelege level
		"creates a test scroll.", //command description
		"/scroll")] //usage
	public class ScrollCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, new InventoryItem(UtilityScrollsEvent.TrainerScroll));
			client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, new InventoryItem(UtilityScrollsEvent.MerchantScroll));
			client.Player.Inventory.AddItem(eInventorySlot.FirstEmptyBackpack, new InventoryItem(UtilityScrollsEvent.HealerScroll));
			return 1;
		}
	}
}

namespace DOL.GS.Trainer
{
	public class GenericTrainer : GameTrainer
	{
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			player.Out.SendTrainerWindow();

			return true;
		}
	}
}