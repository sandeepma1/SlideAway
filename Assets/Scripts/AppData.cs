public static class AppData
{
    public static int currentScore;
    //Achivements
    public const int achievementValue1 = 50;
    public const int achievementValue2 = 100;
    public const int achievementValue3 = 250;

    //Ball variables
    public const float minSpeed = 12;
    public const float maxSpeed = 18;

    //Tile Manager
    public const int gemSpawnRate = 10;//higher is less spawn

    //Rewards
    public const int nextRewardInHours = 6;
    public const int gemsRewards = 25;

    //Misc
    public const float floorSaturation = 0.8f;
    public const float floorLightness = 0.75f;

    public const string shopItemsDbJsonPath = "DB/ShopDB";
    public const string allBallMatPath = "BallMaterials";

    public const string gemIcon = "<sprite=0>";
    public const string adIcon = "<sprite=1>";

    public const string saveVersion = "0.1";
}
