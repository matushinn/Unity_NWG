// Config class
var Config = function(itemId, ncmb) {
    this.itemId = itemId;
    this.ncmb = ncmb;
};

Config.prototype.getItems = function() {
    var ncmb = this.ncmb;
    var itemId = this.itemId;

    return new Promise(function(resolve, reject) {
	var Items = ncmb.DataStore('GachaSetting');
	Items.equalTo('id', itemId)
	    .fetchAll()
	    .then(function(results) {
		var weights = results[0];
		var ignoreKeys = [
		    'objectId'
		    , 'id'
		    , 'createDate'
		    , 'updateDate'
		    , 'acl'
            , 'memo'
		];
		var params = {};
		for (var key in weights) {
		    if (!weights.hasOwnProperty(key) ||
			(ignoreKeys.indexOf(key) != -1)) {
			continue;
		    }
		    params[key] = weights[key];
		}
		resolve(params);
	    })
	    .catch(function(err) {
		reject(err);
	    });
    });
};
module.exports = Config;

// Chooser class
var Chooser = function(items) {
    this.items = [];
    this.total = 0;

    var itemKeys = Object.keys(items);
    for (var i = 0; i < itemKeys.length; ++i) {
	var val = itemKeys[i];
	var weight = items[val];
	this.addItem(val, weight);
    }
}

Chooser.prototype.addItem = function(val, weight) {
    this.items.push({
	val: val
	, weight: weight
    });
    this.total += weight;
}

Chooser.prototype.deleteItem = function(val) {
    var len = this.items.length;
    for (var i = 0; i < len; ++i) {
	if (this.items[i].val === val) {
	    this.items.splice(i, 1); // delete it
	    break;
	}
    }
}

Chooser.prototype.getItem = function(idx) {
    var prev = null;
    for (var item of this.items) {
	if (prev === null) {
	    prev = item;
	}
	if (item.start > idx) {
	    return prev.val;
	}
	prev = item;
    }
    return prev.val;
}

Chooser.prototype.numItems = function() {
    return this.items.length;
}

Chooser.prototype.reset = function() {
    this.items = [];
    this.total = 0;
}

Chooser.prototype.choose = function() {
    this.setup();

    var idx = Math.floor(Math.random() * this.total + 1);
    return this.getItem(idx);
}

Chooser.prototype.setup = function() {
    var start = 0;
    for (var item of this.items) {
	item.start = start;
	start += item.weight;
    }
    this.total = start;
}
module.exports = Chooser;

// Logger class
var Logger = function(ncmb) {
    this.ncmb = ncmb;
}

Logger.prototype.log = function(userId, itemId, item) {
    var logger = this;
    return Promise.all([
	logger.saveLog(userId, itemId, item),
	logger.updateCounter(itemId, item)
    ]);
}

Logger.prototype.updateCounter = function(itemId, item) {
    var logger = this;
    return new Promise(function(resolve, reject) {
	var Counter = logger.ncmb.DataStore('GachaCounter');

	Counter.equalTo('id', itemId)
	    .fetchAll()
	    .then(function(results) {
		var counter = null;
		if (results.length == 0) {
		    counter = new Counter;
		    counter.set('id', itemId)
			.set(item, 1)
			.save()
			.then(resolve);
		} else {
		    counter = results[0];
		    counter.setIncrement(item, 1)
			.update()
			.then(resolve);
		}
	    })
	    .catch(function(err) {
		reject(err);
	    });
    });
}

Logger.prototype.saveLog = function(userId, itemId, item) {
    var logger = this;
    return new Promise(function(resolve, reject) {
	var Log = logger.ncmb.DataStore('GachaLog');
	var log = new Log();
	log.set('userId', userId)
	    .set('itemId', itemId)
	    .set('item', item)
	    .save()
	    .then(function() {
		resolve();
	    })
	    .catch(function(err) {
		reject(err);
	    });
    });
}
module.exports = Logger;


// gacha main
module.exports = function(req, res) {
    var NCMB = require('ncmb');
    var app_key = 'YOUR_APP_KEY';
    var client_key = 'YOUR_CLIENT_KEY';

    if (req.query.user == undefined || req.query.item == undefined) {
	res.status(400).send('Bad Request');
	return;
    }

	var itemArray;
    var userId = req.query.user;
    var itemId = parseInt(req.query.item, 10);
    var ncmb = new NCMB(app_key, client_key);

    var config = new Config(itemId, ncmb);


	config.getItems()
	.then(function(params) {
		var chooser = new Chooser(params);
		var item = chooser.choose();

		var logger = new Logger(ncmb);
		logger.log(userId, itemId, item)
		.then(function() {
			res.send(item);
		})
		.catch(function(err) {
			throw err;
		});
	})
	.catch(function(err) {
		res.status(500).send(err.message);
	});
}
