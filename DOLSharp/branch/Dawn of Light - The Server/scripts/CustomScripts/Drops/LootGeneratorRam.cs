using System;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.Keeps;
using log4net;

namespace DOL.GS
{
	public class LootGeneratorRam : LootGeneratorBase 
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[ScriptLoadedEvent]
		public static void OnScriptLoaded(DOLEvent e, object sender, EventArgs args)
		{
			Spell load = RamSpell;
			LootMgr.RegisterLootGenerator(new LootGeneratorRam(), "", "", "", 165);
			log.Info("Ram Drop System Loaded!");
		}

		protected static Spell m_ramSpell;
		public static Spell RamSpell
		{
			get
			{
				if (m_ramSpell == null)
				{
					DBSpell spell = new DBSpell();
					spell.AutoSave = false;
					spell.CastTime = 0;
					spell.ClientEffect = 0;
					spell.Icon = 0;
					spell.Duration = 0;
					spell.Description = "Summons a Ram at your location, can only be used in RvR.";
					spell.Name = "Summon Ram";
					spell.Range = 0;
					spell.SpellID = 64004;
					spell.Target = "Self";
					spell.Type = "SummonRam";
					spell.Value = 0;
					m_ramSpell = new Spell(spell, 1);
					SkillBase.GetSpellList(GlobalSpellsLines.Item_Effects).Add(m_ramSpell);
				}
				return m_ramSpell;
			}
		}

		protected static ItemTemplate m_ram;
		public static ItemTemplate Ram
		{
			get
			{
				if (m_ram == null)
				{
					m_ram = new ItemTemplate();
					m_ram.CanDropAsLoot = false;
					m_ram.Charges = 1;
					m_ram.Id_nb = "ram_scroll";
					m_ram.IsDropable = true;
					m_ram.IsPickable = true;
					m_ram.IsTradable = true;
					m_ram.Item_Type = 41;
					m_ram.Level = 1;
					m_ram.MaxCharges = 1;
					m_ram.Model = 499;
					m_ram.Name = "Ram Scroll";
					m_ram.Object_Type = (int)eObjectType.Magical;
					m_ram.Realm = 0;
					m_ram.SpellID = RamSpell.ID;
					m_ram.Quality = 99;
					m_ram.Copper = 5;
				}
				return m_ram;
			}
		}

		/// <summary>
		/// Generate loot for given mob
		/// </summary>
		/// <param name="mob"></param>
		/// <returns></returns>
		public override LootList GenerateLoot(GameNPC mob, GameObject killer)
		{
			Spell load = RamSpell;
			LootList loot = base.GenerateLoot(mob, killer);
			if (mob is GameKeepGuard && Util.Chance(10))
				loot.AddFixed(Ram);
			return loot;
		}
	}
}

namespace DOL.GS.Spells
{
	[SpellHandlerAttribute("SummonRam")]
	public class SummonRamSpellHandler : SpellHandler
	{
		public SummonRamSpellHandler(GameLiving caster, Spell spell, SpellLine line)
			: base(caster, spell, line)
		{ }

		public override void ApplyEffectOnTarget(GameLiving target, double effectiveness)
		{
			base.ApplyEffectOnTarget(target, effectiveness);

			int x, y;
			m_caster.GetSpotFromHeading(64, out x, out y);
			GameSiegeRam ram = new GameSiegeRam();
			ram.X = x;
			ram.Y = y;
			ram.Z = m_caster.Z;
			ram.CurrentRegion = m_caster.CurrentRegion;
			ram.Model = 2600;
			ram.Level = 1;
			ram.Name = "light ram";
			ram.Realm = m_caster.Realm;
			ram.AddToWorld();;
		}
	}
}