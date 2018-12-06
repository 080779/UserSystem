// JavaScript Document
function showm(n,h,tlid,boxid,classH,classS){
	for(var i=1;i<=h;i++){
		var curBtn=document.getElementById(tlid+i);
		var curCon=document.getElementById(boxid+i);

		if(n==i){			
			curBtn.className=classS;
			curCon.style.display="block"
		}
		else{
			curBtn.className=classH;	
			curCon.style.display="none"	
		}
		
	}
}