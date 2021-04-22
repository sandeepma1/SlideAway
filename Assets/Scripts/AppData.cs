public static class AppData
{
    //Main variables
    public static int currentScore;
    public static int highScore;
    public static int retries;
    public static int gems;

    //Achivements
    public const int achievementValue1 = 50;
    public const int achievementValue2 = 100;
    public const int achievementValue3 = 250;

    //Ball variables
    public const float minSpeed = 8;
    public const float maxSpeed = 12;

    //Misc
    public const float floorSaturation = 0.8f;
    public const float floorLightness = 0.75f;

    public const string dbPath = "DB/ShopDB";
    public const string gemsBallMatPath = "BallMaterials/GemsBall";
    public const string AdsBallMatPath = "BallMaterials/AdsBall";
    public const string paidBallMatPath = "BallMaterials/PaidBall";

    public const string gemIcon = "<sprite=0>";
    public const string adIcon = "<sprite=1>";
}
