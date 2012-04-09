var Food = {
    foodContext: null,
    foods: [],

    initialize: function () {
        foodContext = document.getElementById('food').getContext('2d');
    },

    addFood: function (x, y) {

        var f = { x: x, y: y, a: Math.floor(Math.random() * 360), s: 1 + Math.floor(Math.random() * 4) };

        Food.foods.push(f);

        if (Food.foods.length === 1) {
            Food.drawFood();
        }
    },

    drawFood: function () {

        foodContext.clearRect(0, 0, Game.width, Game.height);

        for (var i = 0; i < Food.foods.length; i++) {
            var f = Food.foods[i];

            foodContext.save();

            foodContext.translate(f.x, f.y);
            foodContext.rotate(f.a * (Math.PI / 180));

            drawCircle(foodContext, 0, 0, 11, "#ff00ff");

            drawCircle(foodContext, -3, -4, 3, "#ffffff");
            drawCircle(foodContext, 3, -4, 3, "#ffffff");

            drawCircle(foodContext, -4, -4, 1, "#880088");
            drawCircle(foodContext, 4, -4, 1, "#880088");

            foodContext.beginPath();
            foodContext.strokeStyle = '#880088';
            foodContext.arc(0, 0, 11, 0, Math.PI * 2, true);
            foodContext.moveTo(3, 5);
            foodContext.arc(0, 0, 5, 0, Math.PI, false);

            foodContext.stroke();

            foodContext.restore();

            f.a+= f.s;
        }

        if (Food.foods.length > 0) {
            window.setTimeout(function () { Food.drawFood(); }, 40);
        }

    },

    foodEaten: function (x, y) {
        Food.foods = _.filter(Food.foods, function (f) { return f.x !== x && f.y !== y; });
    }
}