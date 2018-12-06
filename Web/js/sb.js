   /**
   *obj 当前对象
   *c_name 类名称
   *v 默认值
   */
   function ChangeColor(obj,c_name,v){ //鼠标单击
   		__this = obj;
		__v = v;
		if(__this.value!==__v) __this.value = __v
   		var n_class = c_name;
		__this.className =n_class; //样式名称
		if(document.addEventListener)//FF
		{
			document.addEventListener("keydown",clear,false);
		}
		else//IE
		{
			document.attachEvent("onkeydown",clear);
		}	
    } 
	
   function reColor(obj,c_name) //失去焦点
   {
	   __this = obj;
	   if(__this.value=='') __this.value=__v;

	   if (__this.className != "input_input_mima3" || __this.value=='') {
	       //样式名称
	       var n_class = c_name;
	       __this.className = n_class;
	   } //样式名称
   }		
	
	function clear(){
		if(__this.value==__v)
		{
			__this.value = '';	
		}
	}