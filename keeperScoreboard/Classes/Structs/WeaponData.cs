using System.Collections.Generic;

namespace keeperScoreboard.Classes.Structs
{
    public class weapons
    {
        public ulong key { get; set; }
        public RootObject weaponData { get; set; }
    }
    public class Req
    {
        public string c { get; set; }
        public string t { get; set; }
        public int v { get; set; }
    }
    public class RootObject
    {
        public string category { get; set; }
        public string rareness { get; set; }
        public string name { get; set; }
        public List<Req> req { get; set; }
        public List<string> see { get; set; }
        public string categoryType { get; set; }
        public string slug { get; set; }
        public string desc { get; set; }
    }
    public class weaponImagesRoot
    {
        public string key { get; set; }
        public weaponImages images { get; set; }
    }
    public class weaponImages
    {
        public string category { get; set; }
        public string slug { get; set; }
        public List<versions> versions {get;set;}
    }
    public class versions
    {
        public string path { get; set; }
        public bool isSprite { get; set; }
        public int height { get; set; }
        public string name { get; set; }
        public int width { get; set; }
    }
    /*
    currentLoadout->kits->kitid
    0	:	669091281		 : Main Weapon
    1	:	37082993		 : Side Arm
    2	:	289218432		 : Gadget 1
    3	:	2887915611       : Gadget 2
    4	:	2670747868       : Grenade
    5	:	3214146841       : Knife
    6	:	3316828753       : Field Upgrades
    7	:	0                : Unknown 
    8	:	0                : Unknown
    9	:	458988977	     : Unknown  
    10	:	2608762737	     : Unknown
    11	:	937188493        : Player Camo
    12	:	3416970831       : Unknown
    Read gun info from currentLoadout->weapons->weaponID

    AEK: Data is in no particular order
    0	:	4268630938	     : weapon sight
    1	:	904792095	     : underbarel e.g laser
    2	:	997651705		 : barel
    3	:	2236604428		 : grip type
    4	:	2691423844		 : cammo
    5	:	0                : ammo type e.g shotguns
*/
}
