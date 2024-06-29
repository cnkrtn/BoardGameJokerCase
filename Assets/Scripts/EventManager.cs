using System;
using System.Collections.Generic;

public static class EventManager 
{
        
    public static Action OnTileConfigurationEnd;
    public static Action OnMapCreationCompleted;
    public static Action<int> OnSimAnimationFinished;
    public static Action<int,int,int> OnStoppedOnACell;
  

}

    

