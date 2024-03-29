mergeInto(LibraryManager.library, {
  IsIOSDevice: function () {
    return (
      ['iPad Simulator', 'iPhone Simulator', 'iPod Simulator', 'iPad', 'iPhone', 'iPod'].includes(navigator.platform) ||
      // iPad on iOS 13 detection
      (navigator.userAgent.includes('Mac') && 'ontouchend' in document)
    );
  },
  GetServerUrlAddress: function () {
    var returnStr = "localhost";

    //if(window.location.origin.indexOf('dev') >= 0)
      //returnStr = "api.sample.dev.hasteoriginals.com";
    //else if(window.location.origin.indexOf('stage') >= 0)
      //returnStr = "api.sample.stage.hasteoriginals.com";
    //else if(window.location.origin.indexOf('rallieon') >= 0)
      //returnStr = "localhost";
    //else if(window.location.origin.indexOf('localhost') >= 0)
      //returnStr = "localhost";
    //else
      returnStr = "william-test-server.deamtest.com";//"haste-racer-game-server.neurokod.com"; //;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  GetToken: function () {
    var returnStr = localStorage.getItem("token");//window.token.token; // comes from HasteLoader.js
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  PerformLogout: function() {
    logoutHaste();
    return true;
  },
  BackToArcade: function() {
    window.location.href = "https://app.hastearcade.com/games/TYz6dV0mn8mu0NMSkzqATGXthrIvEssV6hRojtyjA3o5eoxOjX1iOZosVmTsqV8j";//your-haste-arcade-game-id-uuid
    return true;
  },
  IsLandscape: function() {
    return window.innerHeight < window.innerWidth;
  }
});