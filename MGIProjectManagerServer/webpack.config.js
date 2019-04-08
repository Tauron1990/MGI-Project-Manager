var path = require('path');

module.exports = {
	entry: './wwwroot/src/lib.js',
	output: {
		filename: '[name].bundle.js',
		path: path.resolve(__dirname, 'wwwroot/lib')
	},
	optimization: {
		splitChunks: {
			chunks: 'all'
		}
	}
};