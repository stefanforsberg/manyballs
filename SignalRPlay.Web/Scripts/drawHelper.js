function drawCircle(context, x, y, r, color) {
    context.fillStyle = color;

    context.beginPath();
    context.arc(x, y, r, 0, Math.PI * 2, true);
    context.closePath();

    context.fill();
}