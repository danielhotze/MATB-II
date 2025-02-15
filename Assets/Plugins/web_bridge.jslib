mergeInto(LibraryManager.library, 
{
  SendPerformanceData: function(data)
  {
    var dataStr = UTF8ToString(data);
    console.log("Sending performance data to window: ", dataStr);
    // window.parent.postMessage(data, '*');
    window.receivePerformanceData(dataStr);
  },

  RequestDifficultyLevel: function()
  {
    console.log("Requesting difficulty level from frontend.");
    window.sendDifficultyLevel();
  },

  QuitGame: function() {
    console.log("Quit Game was sent from Unity Instance.");
    window.quitGameInstance();
  }
});