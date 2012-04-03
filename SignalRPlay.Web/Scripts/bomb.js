function Bomb(x, y, xspeed, yspeed, id, img) {
    this.x = x;
    this.y = y;
    this.xspeed = xspeed;
    this.yspeed = yspeed;
    this.id = id;
    this.img = img;
    this.angle = 0;
}

Bomb.prototype.add = function () {
    $("#bomb-area").append("<canvas width=\"500\" height=\"500\" style=\"z-index: 50; top: 35px; left: 5px;\" id=\"bomb-" + this.id + "\"></canvas>");
    this.contextBomb = document.getElementById("bomb-" + this.id).getContext('2d');
    this.contextBomb.drawImage(this.img, this.x, this.y);

    this.x += 22;
    this.y += 22;

    var bomb = this;

    window.setTimeout(function () { bomb.Animate(); }, 55);
};

Bomb.prototype.Animate = function () {
    this.contextBomb.clearRect(0, 0, 500, 500);

    this.contextBomb.drawImage(img, this.x - 22, this.y - 22);

    this.contextBomb.fillStyle = "rgba(255, 0, 0, " + ((this.angle / 500)) + ")";

    this.contextBomb.moveTo(this.x, this.y);
    this.contextBomb.beginPath();
    this.contextBomb.arc(this.x, this.y, 22, 0, (this.angle + 5) * Math.PI / 180, false);
    this.contextBomb.lineTo(this.x, this.y);
    this.contextBomb.fill();
    this.contextBomb.closePath();
    this.angle += 4;

    this.x += this.xspeed;
    this.y += this.yspeed;

    var bomb = this;

    if (this.angle < 360) {
        window.setTimeout(function () { bomb.Animate(); }, 55);
    }
};