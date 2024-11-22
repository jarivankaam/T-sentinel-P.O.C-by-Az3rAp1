const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = {
    entry: './assets/scss/style.scss',
    output: {
        // You can omit JS output as we only care about the CSS output in this case
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'dist'), // This can be any directory for JS files
    },
    module: {
        rules: [
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader, // Extracts CSS into separate file
                    'css-loader',   // Translates CSS into CommonJS
                    'sass-loader'   // Compiles SCSS to CSS
                ],
            },
        ],
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: '../sass-style.css', // Outputs to the root directory
        }),
    ],
    watch: true, // Enable watch mode
    mode: 'development',
};
