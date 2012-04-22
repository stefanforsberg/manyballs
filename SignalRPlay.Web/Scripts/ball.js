function Ball(local, context) {
    this.local = local;
    this.context = context;
    this.tailCounter = 0;
    this.shadeColor = shadeColor(local.Value.Color, -70);
}

Ball.prototype.draw = function (server) {
    var ball = this.local.Value;
    
    this.updateLocal(server);

    context.save();
    this.setupStage(ball);
    this.drawTail(ball);
    this.drawBody(ball);
    this.drawEyes(ball);
    context.restore();
};

Ball.prototype.updateLocal = function(server) {
    var ball_old = this.local.Value;
    var ball_new = server.Value;

    var xpos = ball_old.LocX;
    var ypos = ball_old.LocY;
    var size = ball_old.Size;

    this.local.Value.Color = server.Value.Color;
    this.local.Value.LastDir = server.Value.LastDir;

    if (size < ball_new.Size) {
        this.local.Value.Size++;
    } else if (size > ball_new.Size) {
        this.local.Value.Size--;
    }

    this.local.Value.LocX += this.differ(xpos, ball_new.LocX);
    this.local.Value.LocY += this.differ(ypos, ball_new.LocY);
};

Ball.prototype.drawTail = function (ball) {
    var tail = [
        {
            size: ball.Size / 3,
            x: -(ball.Size - (ball.Size / 10)),
            y: 3 * Math.sin((this.tailCounter + 45) / 20)
        },
        {
            size: ball.Size / 6,
            x: -(ball.Size - (ball.Size / 10) + (ball.Size / 3)),
            y: 3 * Math.sin((this.tailCounter) / 20)
        }
    ];

    drawCircle(context, tail[0].x, tail[0].y, tail[0].size, this.shadeColor);
    drawCircle(context, tail[1].x, tail[1].y, tail[1].size, this.shadeColor);

    this.tailCounter+=3;
};

Ball.prototype.drawEyes = function(ball) {
    var eyeSize = ball.Size / 10;

    var eyePosition = ball.Size / 2;
    var eyeDistance = ball.Size / 3;

    var leftEye = { x: eyePosition, y: eyeDistance };
    var rightEye = { x: eyePosition, y: -eyeDistance };

    drawCircle(context, leftEye.x, leftEye.y, eyeSize + 1, this.shadeColor);
    drawCircle(context, rightEye.x, rightEye.y, eyeSize + 1, this.shadeColor);

    drawCircle(context, leftEye.x, leftEye.y, eyeSize, "#ffffff");
    drawCircle(context, rightEye.x, rightEye.y, eyeSize, "#ffffff");
};

Ball.prototype.setupStage = function(ball) {
    context.translate(ball.LocX, ball.LocY);

    if (ball.LastDir === "l") {
        context.rotate(Math.PI);
    }
    if (ball.LastDir === "u") {
        context.rotate((Math.PI * 3) / 2);
    }
    if (ball.LastDir === "d") {
        context.rotate(Math.PI / 2);
    }
};

Ball.prototype.drawBody = function(ball) {
    drawCircle(context, 0, 0, ball.Size, this.shadeColor);
    drawCircle(context, 0, 0, ball.Size - 1, ball.Color);
};

Ball.prototype.differ = function (a1, a2) {
    var total = 0;

    var diffDirection = 0;

    if (a1 < a2) {
        diffDirection = 1;
    }
    else if (a1 > a2) {
        diffDirection = -1;
    }

    var absDiff = Math.abs(a1 - a2);

    if (absDiff < 4) {
        total = diffDirection * (absDiff * 0.1);
    }
    else if (absDiff >= 4 && absDiff < 15) {
        total = diffDirection * (absDiff * 0.3);
    }
    else {
        total = absDiff * diffDirection;
    }

    return total;
}