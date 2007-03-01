/*
 * Author:		Kristopher Gilbert <ogre@fallenrealms.net>
 * Rev:			$Id: Location.cs 354 2005-04-05 10:41:33Z ogre $
 * Copyright:	2004 by Hired Goons, LLC
 * License:		http://www.gnu.org/licenses/gpl.txt
 * 
 * Utility object that stores locations.
 * 
 */

using DOL.GS ;

namespace DOL.GS.GameEvents
{
	public interface ITeleportLocation
	{
		string Name { get;set;}
	}

	public class Location : GameObject, ITeleportLocation
	{
		int tempX, tempY;
		public Location(string name, ushort region, int x, int y, int z, ushort heading)
			: base()
		{
			Name = name;
			CurrentRegionID = region;
			X = x;
			Y = y;
			Z = z;
			Heading = heading;
		}
		public void getDestinationSpot(out int destinationX, out int destinationY)
		{
			ushort originalHeading = Heading;
			Heading = (ushort)Util.Random((Heading - 500), (Heading + 500));
			int distance = Util.Random(50, 150);
			GetSpotFromHeading(distance, out tempX, out tempY);
			destinationX = tempX;
			destinationY = tempY;
			Heading = originalHeading;
		}
	}

	public class SpecialLocation : ITeleportLocation
	{
		public enum eSpecialLocation
		{
			PersonalHouse,
			GuildHouse,
			BindHouse,
			BindPoint,
		}

		public eSpecialLocation Location;
		private string m_name;
		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public SpecialLocation(string name, eSpecialLocation location)
		{
			Location = location;
			Name = name;
		}
	}

	public class LocationExpansion : Location
	{
		//TODO: Convert to accessors
		public GameClient.eClientType Software;
		public GameClient.eClientAddons Expansions;
		public GameClient.eClientVersion Version;
		public byte MinLevel = 0;
		public byte MaxLevel = 0;

		public LocationExpansion (	string name ,
									ushort region ,
									int x , int y , int z ,
									ushort heading ,
									GameClient.eClientType software ,
									GameClient.eClientAddons expansions,
									GameClient.eClientVersion version)
			: base ( name , region, x , y , z , heading )
		{
			Software = software ;
			Expansions = expansions ;
			Version = version;
		}

		public LocationExpansion(string name,
							ushort region,
							int x, int y, int z,
							ushort heading,
							GameClient.eClientType software,
							GameClient.eClientAddons expansions,
							GameClient.eClientVersion version,
							byte minLevel, byte maxLevel)
			: base(name, region, x, y, z, heading)
		{
			Software = software;
			Expansions = expansions;
			Version = version;
			MinLevel = minLevel;
			MaxLevel = maxLevel;
		}
	}
}
