mergeInto(LibraryManager.library, 
{
  SendPerformanceData: function(data)
  {
    console.log("Sending performance data to window: ", data);
    // window.parent.postMessage(data, '*');
    window.receivePerformanceData(data);
  },

  RequestDifficultyLevel: function()
  {
    console.log("Requesting difficulty level from frontend.");
    window.sendDifficultyLevel()
  }
});