mergeInto(LibraryManager.library, 
{
   WebglAsherLink3DStart: function ()
  {
    if(typeof AsherLink3DStart != "undefined" && AsherLink3DStart instanceof Function){
        AsherLink3DStart();
    }
  },
   WebglAsherLink3DClickMessage: function(str)
  {
    if(typeof AsherLink3DClickMessage != "undefined" && AsherLink3DClickMessage instanceof Function){
        AsherLink3DClickMessage(Pointer_stringify(str));
    }
  },
   WebglAsherLink3DLongClickMessage: function(str)
  {
    if(typeof AsherLink3DLongClickMessage != "undefined" && AsherLink3DLongClickMessage instanceof Function){
        AsherLink3DLongClickMessage(Pointer_stringify(str));
    }
  },
   WebglAsherLink3DConfig: function(str)
  {
    if(typeof AsherLink3DConfig != "undefined" && AsherLink3DConfig instanceof Function){
        AsherLink3DConfig(Pointer_stringify(str));
    }
  },
  HelloString: function (str) 
  {
    window.alert(Pointer_stringify(str));
  },
  WebglAsherLink3DWebLog: function(str)
 {
    console.log(Pointer_stringify(str))
 },
 WebglHelloFloat: function()
  {
  var userAgentInfo = navigator.userAgent;      
    var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];    
    var flag = true;       
    for (var v = 0; v < Agents.length; v++)
    {        
      if (userAgentInfo.indexOf(Agents[v]) > 0)
      {   
        return 1; //如果是手机端就返回1
        break;       
      }     
    }       
        return 2; //如果是pc就返回2
  },
 });
