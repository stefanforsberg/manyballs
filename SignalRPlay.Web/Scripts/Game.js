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

            if (size < ball_new.Size) {
                current_user_data[i].Value.Size++;
            }
            else if (size > ball_new.Size) {
                current_user_data[i].Value.Size--;
            }

            if (xpos < ball_new.LocX) {
                current_user_data[i].Value.LocX++;
            }
            else if (xpos > ball_new.LocX) {
                current_user_data[i].Value.LocX--;
            }

            if (ypos < ball_new.LocY) {
                current_user_data[i].Value.LocY++;
            }
            else if (ypos > ball_new.LocY) {
                current_user_data[i].Value.LocY--;
            }

            drawCircle(context, current_user_data[i].Value.LocX, current_user_data[i].Value.LocY, current_user_data[i].Value.Size, "#000000");
            drawCircle(context, current_user_data[i].Value.LocX, current_user_data[i].Value.LocY, current_user_data[i].Value.Size - 1, ball_new.Color);
        }

        window.setTimeout(function () { Game.updateCanvas(); }, 10);
    },

    joined: function (data) {
        current_user_data = data;
        new_user_data = data;

        window.setTimeout(function () { Game.updateCanvas(); }, 10);
    },

    draw: function (data) {
        new_user_data = data;

        //                context.clearRect(0, 0, 500, 500);

        //                for (var i = 0; i < data.length; i++) {
        //                    var ball = data[i].Value;

        //                    drawCircle(context, ball.LocX, ball.LocY, ball.Size, "#000000");
        //                    drawCircle(context, ball.LocX, ball.LocY, ball.Size - 1, ball.Color);
        //                }
    },

    newBomb: function (x, y, xspeed, yspeed, bombId) {
        $("#bomb-area").append("<canvas width=\"500\" height=\"500\" style=\"z-index: 50; top: 35px; left: 5px;\" id=\"bomb-" + bombId + "\"></canvas>");

        var bombCanvas = document.getElementById("bomb-" + bombId);
        var contextBomb = bombCanvas.getContext('2d');

        contextBomb.drawImage(img, x, y);
        window.setTimeout(function () { Game.coolDown(x + 22, y + 22, xspeed, yspeed, 0, contextBomb); }, 55);
    },

    coolDown: function (x, y, xspeed, yspeed, angle, contextBomb) {
        contextBomb.clearRect(0, 0, 500, 500);

        contextBomb.drawImage(img, x - 22, y - 22);

        contextBomb.fillStyle = "rgba(255, 0, 0, " + ((angle / 500)) + ")";

        contextBomb.moveTo(x, y);
        contextBomb.beginPath();
        contextBomb.arc(x, y, 22, 0, (angle + 5) * Math.PI / 180, false);
        contextBomb.lineTo(x, y);
        contextBomb.fill();
        contextBomb.closePath();
        angle += 4;
        if (angle < 360) {
            window.setTimeout(function () { Game.coolDown(x + xspeed, y + yspeed, xspeed, yspeed, angle, contextBomb); }, 55);
        }
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