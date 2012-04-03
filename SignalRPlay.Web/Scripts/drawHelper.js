function drawCircle(context, x, y, r, color) {
    context.fillStyle = color;

    context.beginPath();
    context.arc(x, y, r, 0, Math.PI * 2, true);
    context.closePath();

    context.fill();
}

function shadeColor(color, shade) {
    var colorInt = parseInt(color.substring(1), 16);

    var R = (colorInt & 0xFF0000) >> 16;
    var G = (colorInt & 0x00FF00) >> 8;
    var B = (colorInt & 0x0000FF) >> 0;

    R = R + Math.floor((shade / 255) * R);
    G = G + Math.floor((shade / 255) * G);
    B = B + Math.floor((shade / 255) * B);

    var newColorInt = (R << 16) + (G << 8) + (B);
    var newColorStr = "#" + newColorInt.toString(16);

    return newColorStr;
}