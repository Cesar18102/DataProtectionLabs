<?php
	
	$privateKeyData = "-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA0HARdxwRDxDMON1pNEcb7hBwPgexnPocGIE/70H/36/T2KKh
8puF5erXEp2HDOtygGTiwRvOyX6ooA48Fo2Wm6zXWPGn4NDL6YU0v3tJdmPQ9V0C
mS4BirZS2nBg5G+alrXSXDq5W4TsO7K7OVTyFLVoa9LmnVHw1EKehzj1Xg5vq5Vs
3xTt0Gl7oI/ix+3FXORrW+5mQ46ORWPgW4zcHdQD18mOhLfofsoXKRmLK+ha1uz2
EqN8Cdxc3XIrxJZ7Qp69jx2SVva/+kLfXkZVhGIIe5A+M/okQjY4IDK3eRdaDAqc
iQL0vl1VrLtFPB0SvFej+79LlZexXVVMA4jMNQIDAQABAoIBACgQcnr3jRvdObPw
cwlWZbrzs+9oYS5wMLhRF63wfLNNQ6YPVJuiYzFXW3G7j7rfAspFGgTOSIgYbWce
dVRMcNeJeHcjNAaVe6xuKz5m/y7fbsnKz/YLjsZP10sfcLBX91nipwWNUBWUTigH
ac9SUZnDaMfY6PTyTB0wlPIe86LZQEDz+CybLD9NKbEw0WV9VkctC+Tjmv4t3gkw
UIhf6+VXxvhZyEMhX86TfWzCS8O0fniWJAf2H04+cSHYEnAegnNJKZXlu+zKnUna
IooR/3bTAcgS6P3aKEX+yAyzUs3o6RzApgup5rkPUx6H0LU0juSBG/kVHDwCM0dm
Y8KwjkECgYEA/6/eZajMAFevvtf8mK1lxl/vVsduM8+eKiZLMfBLcLAB1tmb5Y/O
ZpGrz4ugwiwd8JdRR1x2ZhucK9tF1c1SDiWsUYWfTaltFvSBWSO4+uKkoOtL2Le9
xOU7G3J4ch0X6WGJ+lz2Z4EaalXL0v0IHWWQrgJWp294sLYUjjZaGNECgYEA0LFk
SyNGxsSulKPIGF5kjwWrRLRgVYiW3fPX63k1Lhx6PBwt2cr+n6ile+Y7N+CsB5UC
pJv/dOje1wkuI4WleVS4HrWjDiR5IveVLFGWNCUdK9WskTQRfWqyhRW1kPQdsxYN
jWqpwdWORU8xoZOeAowVIYqW7dCYlcQjOuydViUCgYBTj3xVKOcSrZWH/zpUA0ch
n7SuXZrMX+vGxqEIfwmImFzhhFiIm4jOyIin2W/+RUC7ohg0BCnFkPxm8NBP4TwR
14oLUmoPjXFTcTDcmuoXQ/6dgIhXGSVxtaCthamLUarA4Nmg8sSsaurhGYz1NTBz
uv+ffrAnJC0DQ15QY77qIQKBgQCvxEhBVHO+41ZakFrt00qEfvTRx82/HGxcgYxg
odRdgtScnessHOE5XVQLN0y28j5RoHn6Y/zS3XMZz3yRpHVK7ESqcLblKWjxP27f
RdPpFT0tdyiBn0X/yIaUukUS0dfVxoQhkC1ruM67p0iY2nhhktQVTOFzaJWqxega
L8EQFQKBgEQFmuptTPOTVf501ljm3r4vDP1+hR5b6H2LoHEqquX6hpzIQRQg6wJW
9nCGUJj4dgPScpGO6rqrdYOYDrvUM4YOslcbCDAxTPy7b3ZejRW05s6Urry/E9Fs
RYCoVgVVhto1f8NgwO9XF2RwaqW4xI6+sh5i6KcZffKvXBYU8Y+x
-----END RSA PRIVATE KEY-----
";

	$data = file_get_contents('php://input');
	$json = json_decode($data, true);


	if(!isset($json["login"])) {
		http_response_code(400);
		echo "{ \"status\" : \"fail\", \"message\" : \"login is required\" }";
		return;
	}
	
	if(!isset($json["password"])) {
		http_response_code(400);
		echo "{ \"status\" : \"fail\", \"message\" : \"password is required\" }";
		return;
	}

	$login = $json["login"];
	$passwordEncrypted = base64_decode($json["password"]);
	$privateKey = openssl_get_privatekey($privateKeyData);

	if(!openssl_private_decrypt($passwordEncrypted, $passwordDecrypted, $privateKey)) {
		http_response_code(400);
		echo "{ \"status\" : \"fail\", \"message\" : \"decryption failed\" }";
		return;
	}
	
	if(!preg_match("/^[A-Za-z0-9_]{8,20}$/", $passwordDecrypted)) {
		http_response_code(400);
		echo "{ \"status\" : \"fail\", \"message\" : \"invalid password\", \"data\" : \"$passwordDecrypted\" }";
		return;
	}
	
	$password = md5($passwordDecrypted);
	
	$db = mysqli_connect("localhost", "root", "", "rsa_test");
	mysqli_query($db, "INSERT INTO accounts VALUES(0, \"$login\", \"$password\");");
	echo "{ \"status\" : \"success\", \"message\" : \"registration successful\" }";
?>