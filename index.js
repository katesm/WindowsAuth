var edge = require('edge');//Require edge...
var auth = edge.func(require('path').join(__dirname, 'ADLogin.cs')); //Add the ADLogin file
var express = require('express');
var bodyParser = require('body-parser');
var app = express();
//Get the deployPath
var deployPath = process.env.deployPath || "";
//To process body on post
app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

/*
    HTTP POST 
    Body of post must include a username and password
    
    @returns boolean


 */
app.post(deployPath + '/auth', function (req, res) {
   
    console.log(req.body);
    auth({username: req.body.username, password:req.body.password},function(err, result){
        console.log(err);
        console.log(result);
        if(err) res.send({success:false, message:'There was a problem'});

        if(result){
            
            return res.send({ success: true, message: 'User passed auth' });
        
        }


         return res.send({ success: false, message: 'User failed auth' });
        
    });
});

var server = app.listen(process.env.PORT || 8081, function () {
    var host = server.address().address
    var port = server.address().port

    console.log("Example app listening at http://%s:%s", host, port)
})

