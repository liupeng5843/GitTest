;(function (doc, win) {
	var docEl = doc.documentElement,
	resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
	recalc = function () {
		var clientWidth = docEl.clientWidth;
		if (!clientWidth) return;
		var fontSize = 20 * (clientWidth / 640);
		docEl.style.fontSize = fontSize + "px"; // (fontSize>20) ? 20 + 'px' : (fontSize + 'px');
	};
	if (!doc.addEventListener) return;
	win.addEventListener(resizeEvt, recalc, false);
	doc.addEventListener('DOMContentLoaded', recalc, false);
	doc.body.addEventListener('touchstart', function () { }, false);
})(document,window);