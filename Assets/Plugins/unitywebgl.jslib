mergeInto(LibraryManager.library, 
{
   WebglAsherLink3DStart: function ()
  {
    console.log("scene3d start!")
  },
  WebglGetHass:function()
  {
    if(Module.SystemInfo.hass  != "undefined")
    {
        var str = JSON.stringify(Module.SystemInfo.hass.states);
        if(str ==null) return null;
		var buffersize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(buffersize);
        stringToUTF8(str, buffer, buffersize);
        return buffer;
    }
    return null;
  },
   WebglAsherLink3DClickMessage: function(str)
  {
	if(Module.SystemInfo.hass  != "undefined")
    {
		var obj = JSON.parse(Pointer_stringify(str));
		Module.SystemInfo.hass.callService(obj["head"],obj["cmd"],{"entity_id": obj["entity_id"]});
    }
  },
   WebglAsherLink3DLongClickMessage: function(str)
  {
    if(Module.SystemInfo.hassCard != "undefined"){
		var obj = JSON.parse(Pointer_stringify(str));
		const node = Module.SystemInfo.hassCard;
        options = {};
        detail = { entityId: obj["entity_id"] };
        const e = new Event('hass-more-info', {
            bubbles: options.bubbles === undefined ? true : options.bubbles,
            cancelable: Boolean(options.cancelable),
            composed: options.composed === undefined ? true : options.composed
            });
        e.detail = detail;
        node.dispatchEvent(e);
    }
  },
   WebglAsherLink3DConfig: function(str)
  {
	if(Module.SystemInfo.hass != "undefined"){
		var obj = JSON.parse(Pointer_stringify(str));
		Module.SystemInfo.hass.callService('3dscene','configscene',{'data': obj});
	}
  },
   WebglAsherLinkUIConfig: function(str)
  {
	if(Module.SystemInfo.hass != "undefined"){
		var obj = JSON.parse(Pointer_stringify(str));
		Module.SystemInfo.hass.callService('3dscene','configui',{'data': obj});
	}
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
        LoadFile:function (gameobjectName,fitter) {
            var gameobjectNameStr=Pointer_stringify(gameobjectName);
            var fitterStr=Pointer_stringify(fitter);
            console.log("GetSelectFileURL");
            console.log("需要被发送的物体名称转化：");
            console.log(gameobjectNameStr);
            
            console.log("需要筛选文件类型转化：");
            console.log(fitterStr);
            var fileInput = document.getElementById("load");//通过id获取创建好的input元素
            if(!fileInput){
                var fileInput = document.createElement('input');//创建input元素
                console.log("创建input元素");
                fileInput.setAttribute('id', "load");//给创建的input设置id
                
                fileInput.setAttribute('type', 'file');
                fileInput.setAttribute('style','display:none;');
                fileInput.setAttribute('style','visibility:hidden;'); 
                fileInput.setAttribute('multiple', '');
                fileInput.disabled=true;//使input元素不可点击，否则会导致页面上所有位置都可唤起文件对话框
                document.body.appendChild(fileInput);
            }
            
            fileInput.accept=fitterStr;
            fileInput.disabled=false;//设置input可点击，否则无法通过click()打开文件对话框
            fileInput.click()//打开文件对话框
            fileInput.onclick = function (event) {
                this.value = null;
            };
            fileInput.onchange = function (event) {
               //如果需要在unity中获取文件内容，在onchange中获取后通过SendMessage()传回给unity
               var files = event.target.files;
               var res={
                    Path:URL.createObjectURL(files[0]),
                    FileName:files[0].name
               };
               gameInstance.SendMessage(gameobjectNameStr, 'FileDialogResult', JSON.stringify(res));
            }
            document.onmouseup = function() {
                fileInput.click();
                document.onmouseup = null;
            }
            fileInput.disabled=true;//结束时再次设置input不可点击，保证点击其他位置无法打开文件对话框
            //确保只有点击按钮2时才能打开文件对话框
            
        },
 });
