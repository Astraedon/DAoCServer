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
	public class Location : GameObject
	{
		int tempX , tempY ;
		public Location ( string name , ushort region , int x , int y , int z , ushort heading ) : base()
		{
			Name = name ;
			CurrentRegionID = region ;
			X = x ;
			Y = y ;
			Z = z ;
			Heading = heading ;
		}
		public void getDestinationSpot ( out int destinationX, out int destinationY )
		{
			ushort originalHeading = Heading ;
			Heading = (ushort)Util.Random ( ( Heading - 500 ) , ( Heading + 500 ) );
            int distance = Util.Random(50, 150);
			GetSpotFromHeading ( distance , out tempX , out tempY ) ;
			destinationX = tempX ;
			destinationY = tempY ;
			Heading = originalHeading ;
		}
	}
	public class LocationExpansion : Location
	{
		//TODO: Convert to accessors
		public GameClient.eClientType Software;
		public GameClient.eClientAddons Expansions;
		public GameClient.eClientVersion Version;

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
	}
}
