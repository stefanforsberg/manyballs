var Game = {
    gameServer: null,
    context: null,
    img: null,
    current_user_data: null,
    new_user_data: null,

    initialize: function (c) {
        context = c;
        img = new Image();
        img.src = '../MrBomb.png';
    },

    updateCanvas: function () {
        context.clearRect(0, 0, 500, 500);

        for (var i = 0; i < new_user_data.length; i++) {

            if (i >= current_user_data.length) {
                current_user_data.push(new_user_data[i]);
            }

            var ball_old = current_user_data[i].Value;
            var ball_new = new_user_data[i].Value;

            var xpos = ball_old.LocX;
            var ypos = ball_old.LocY;
            var size = ball_old.Size;

            current_user_data[i].Value.Color = new_user_data[i].Value.Color;
            current_user_data[i].Value.LastDir = new_user_data[i].Value.LastDir;

            if (size < ball_new.Size) {
                current_user_data[i].Value.Size++;
            }
            else if (size > ball_new.Size) {
                current_user_data[i].Value.Size--;
            }

            current_user_data[i].Value.LocX += Game.differ(xpos, ball_new.LocX);
            current_user_data[i].Value.LocY += Game.differ(ypos, ball_new.LocY);

            Game.drawBall(current_user_data[i]);
        }

        window.setTimeout(function () { Game.updateCanvas(); }, 10);
    },

    drawBall: function (ball) {
        var shade = shadeColor(ball.Value.Color, -70);
        var eyeSize = ball.Value.Size / 10;

        var eyePosition = ball.Value.Size / 2;
        var eyeDistance = ball.Value.Size / 3;

        var leftEye = { x: ball.Value.LocX, y: ball.Value.LocY };
        var rightEye = { x: ball.Value.LocX, y: ball.Value.LocY };

        drawCircle(context, ball.Value.LocX, ball.Value.LocY, ball.Value.Size, shade);
        drawCircle(context, ball.Value.LocX, ball.Value.LocY, ball.Value.Size - 1, ball.Value.Color);

        if (ball.Value.LastDir === "r") {
            leftEye.x += eyePosition;
            leftEye.y += eyeDistance;

            rightEye.x += eyePosition;
            rightEye.y -= eyeDistance;
        }
        if (ball.Value.LastDir === "l") {
            leftEye.x -= eyePosition;
            leftEye.y += eyeDistance;

            rightEye.x -= eyePosition;
            rightEye.y -= eyeDistance;
        }
        if (ball.Value.LastDir === "u") {
            leftEye.x -= eyeDistance;
            leftEye.y -= eyePosition;

            rightEye.x += eyeDistance;
            rightEye.y -= eyePosition;
        }
        if (ball.Value.LastDir === "d") {
            leftEye.x -= eyeDistance;
            leftEye.y += eyePosition;

            rightEye.x += eyeDistance;
            rightEye.y += eyePosition;
        }

        drawCircle(context, leftEye.x, leftEye.y, eyeSize + 1, shade);
        drawCircle(context, rightEye.x, rightEye.y, eyeSize + 1, shade);
        drawCircle(context, leftEye.x, leftEye.y, eyeSize, "#ffffff");
        drawCircle(context, rightEye.x, rightEye.y, eyeSize, "#ffffff");
    },

    differ: function (a1, a2) {
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
    },

    joined: function (data) {
        current_user_data = data;
        new_user_data = data;

        window.setTimeout(function () { Game.updateCanvas(); }, 10);
    },

    draw: function (data) {
        new_user_data = data;
    },

    newBomb: function (x, y, xspeed, yspeed, bombId) {
        var bomb = new Bomb(x, y, xspeed, yspeed, bombId, img);
        bomb.add();
    },

    newBombExplode: function (x, y, bombId) {
        $("#bomb-" + bombId).remove();
    },

    showUsers: function (data) {
        $("#users ul").empty();

        for (var i = 0; i < data.length; i++) {
            var ball = data[i].Value;

            $("#users ul").append('<li><span style="padding-right: 15px; margin-right: 5px; width: 15px; background-color: ' + ball.Color + '"></span> ' + ball.Name + '</li>');
        }
    },

    updateLog: function (message) {
        $("#log ul").prepend('<li>' + message + '</li>');
    }
};