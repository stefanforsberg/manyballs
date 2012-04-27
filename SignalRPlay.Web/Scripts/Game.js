var Game = {
    gameServer: null,
    context: null,
    img: null,
    current_user_data: null,
    new_user_data: null,
    balls: [],
    width: 800,
    height: 500,

    initialize: function (c) {
        context = c;
        img = new Image();
        img.src = '../MrBomb.png';
    },

    updateCanvas: function () {
        context.clearRect(0, 0, Game.width, Game.height);

        for (var i = 0; i < new_user_data.length; i++) {

            if (i >= current_user_data.length) {
                current_user_data.push(new_user_data[i]);
            }

            if (i >= Game.balls.length) {
                Game.balls.push(new Ball(new_user_data[i], context));
            }

            Game.balls[i].draw(new_user_data[i]);
        }

        Food.drawFood();

        window.setTimeout(function () { Game.updateCanvas(); }, 40);
    },

    joined: function (data) {

        current_user_data = data;
        new_user_data = data;

        Game.updateCanvas();

        //        window.setTimeout(function () { Game.updateCanvas(); }, 40);
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

            $("#users ul").append('<li><span style="padding-right: 15px; margin-right: 5px; width: 15px; background-color: ' + ball.Color + '"></span> ' + ball.Name + '(' + ball.Score + ')</li>');
        }
    },

    updateLog: function (message) {
        $("#log ul").prepend('<li>' + message + '</li>');
    }
};