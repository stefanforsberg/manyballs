var Food = {
    foodContext: null,

    initialize: function () {
        foodContext = document.getElementById('food').getContext('2d');
    },

    addFood: function (x, y) {
        drawCircle(foodContext, x, y, 12, "#ff00ff");
        drawCircle(foodContext, x, y, 9, "#3488bb");
        drawCircle(foodContext, x, y, 6, "#237834");
        drawCircle(foodContext, x, y, 3, "#449900");
    },

    foodEaten: function (x, y) {
        console.log("adasdsadsadsad");
        drawCircle(foodContext, x, y, 13, "#ffffff");
    }
}