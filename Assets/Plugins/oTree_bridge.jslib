mergeInto(LibraryManager.library, 
{
  ReceivePerformanceData: function(data)
  {
    console.log("Sending performance data to window: ", data);
    // window.parent.postMessage(data, '*');
    window.parent.receivePerformanceData(data);
  },
});