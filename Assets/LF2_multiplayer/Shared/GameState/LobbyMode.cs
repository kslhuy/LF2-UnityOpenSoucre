    /// <summary>
    /// Conceptual modes or stages that the lobby can be in. We don't actually
    /// bother to keep track of what LobbyMode we're in at any given time; it's just
    /// an abstraction that makes it easier to configure which UI elements should
    /// be enabled/disabled in each stage of the lobby.
    /// </summary>
    public enum LobbyMode
    {
        ChooseChamp, // "Choose your seat!" stage
        ChampChosen, // "Waiting for other players!" stage

        // Huy add new
        ChooseBackGround, // "Choose your BackGround!" stage , only host can choose background
                        // Can not unlock in this mode
        ChooseAI,

        // CloseLoby,
        LobbyEnding, // "Get ready! Game is starting!" stage
        FatalError, // "Fatal Error" stage
    }


  
