$(document).ready(function () {

    var lastKey = 0;
    var drawingCanvas = document.getElementById('myDrawing');
    var context = drawingCanvas.getContext('2d');

    $("#color").miniColors();

    var gameServer = $.connection.game;

    jQuery.getScript("../../Scripts/game.js",
        function () {

            $.connection.hub.start(function () {

            });

            Game.initialize(context);
            Food.initialize(context);

            gameServer.joined = Game.joined;
            gameServer.draw = Game.draw;
            gameServer.newBomb = Game.newBomb;
            gameServer.newBombExplode = Game.newBombExplode;
            gameServer.showUsers = Game.showUsers;
            gameServer.updateLog = Game.updateLog;
            gameServer.addFood = Food.addFood;
            gameServer.foodEaten = Food.foodEaten;


        });

    $("#join").click(function () {
        $(this).attr('disabled', 'disabled');
        gameServer.join($("#color").val());
        $("#game-area").show();
    });

    $("html").keydown(function (event) {
        var kc = event.which;
        if (lastKey == kc) {
            return;
        }

        lastKey = kc;

        if (kc == "32" || kc == "87" || kc == "65" || kc == "68" || kc == "83") {
            gameServer.handleInput(kc, 1);
        }
    });

    $("html").keyup(function (event) {

        lastKey = 0;

        var kc = event.which;
        if (kc == "87" || kc == "65" || kc == "68" || kc == "83") {
            gameServer.handleInput(kc, 0);
        }
    });
});