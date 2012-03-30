$(document).ready(function () {
    

    var drawingCanvas = document.getElementById('myDrawing');
    var context = drawingCanvas.getContext('2d');

    $("#color").miniColors();

    var gameServer = $.connection.game;

    jQuery.getScript("../../Scripts/game.js",
        function () {

            $.connection.hub.start(function () {

            });
            
            Game.initialize(context);

            gameServer.draw = Game.draw;
            gameServer.newBomb = Game.newBomb;
            gameServer.newBombExplode = Game.newBombExplode;
            gameServer.showUsers = Game.showUsers;
            gameServer.updateLog = Game.updateLog;

            

        });

    //    var game = $.connection.game;
    //    
    //    $.connection.hub.start(function () {

    //    });

    //    game.draw = function (data) {
    //        context.clearRect(0, 0, 500, 500);

    //        for (var i = 0; i < data.length; i++) {
    //            var ball = data[i].Value;

    //            drawCircle(context, ball.LocX, ball.LocY, ball.Size, "#000000");
    //            drawCircle(context, ball.LocX, ball.LocY, ball.Size - 1, ball.Color);
    //        }
    //    };

    //    game.newBomb = function (x, y, xspeed, yspeed, bombId) {
    //        $("#bomb-area").append("<canvas width=\"500\" height=\"500\" style=\"z-index: 50; top: 35px; left: 5px;\" id=\"bomb-" + bombId + "\"></canvas>");

    //        var bombCanvas = document.getElementById("bomb-" + bombId);
    //        var contextBomb = bombCanvas.getContext('2d');

    //        contextBomb.drawImage(img, x, y);
    //        window.setTimeout(function () { coolDown(x + 22, y + 22, xspeed, yspeed, 0, contextBomb); }, 55);
    //    };

    //    var coolDown = function (x, y, xspeed, yspeed, angle, contextBomb) {
    //        contextBomb.clearRect(0, 0, 500, 500);

    //        contextBomb.drawImage(img, x - 22, y - 22);

    //        contextBomb.fillStyle = "rgba(255, 0, 0, " + ((angle / 500)) + ")";

    //        contextBomb.moveTo(x, y);
    //        contextBomb.beginPath();
    //        contextBomb.arc(x, y, 22, 0, (angle + 5) * Math.PI / 180, false);
    //        contextBomb.lineTo(x, y);
    //        contextBomb.fill();
    //        contextBomb.closePath();
    //        angle += 4;
    //        if (angle < 360) {
    //            window.setTimeout(function () { coolDown(x + xspeed, y + yspeed, xspeed, yspeed, angle, contextBomb); }, 55);
    //        }
    //    };

    //    game.newBombExplode = function (x, y, bombId) {
    //        $("#bomb-" + bombId).remove();
    //    };

    //    game.showUsers = function (data) {
    //        $("#users ul").empty();

    //        for (var i = 0; i < data.length; i++) {
    //            var ball = data[i].Value;

    //            $("#users ul").append('<li><span style="padding-right: 15px; margin-right: 5px; width: 15px; background-color: ' + ball.Color + '"></span> ' + ball.Name + '</li>');
    //        }
    //    };

    //    game.updateLog = function (message) {
    //        $("#log ul").prepend('<li>' + message + '</li>');
    //    };

    $("#join").click(function () {
        $(this).attr('disabled', 'disabled');
        gameServer.join($("#color").val());
        $("#game-area").show();
    });

    $("html").keydown(function (event) {
        var kc = event.which;
        if (kc == "32" || kc == "87" || kc == "65" || kc == "68" || kc == "83") {
            gameServer.handleInput(kc);
        }
    });


});