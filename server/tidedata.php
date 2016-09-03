<?php
$nextupdate = @file_get_contents("tidedata.nexttime");
if($nextupdate < time()) {
	update();
}

echo file_get_contents("tidedata.json");

function update() {
	$url = 'http://www.comune.venezia.it/archivio/EN/2104';
	//$url = 'http://www.comune.venezia.it/flex/cm/pages/ServeBLOB.php/L/EN/IDPagina/2104';
	$ch = curl_init($url);
	curl_setopt($ch, CURLOPT_HEADER, 0);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER , true);
	$data = curl_exec($ch);
	curl_close($ch);
	
	/*
	$data = substr($data, strpos($data, "<!-- Begin BLOB Content -->"));
	$data = substr($data, 0, strpos($data, "<!-- End BLOB Content -->"));
	$data = strip_tags($data);
	
	$data = substr($data, strpos($data, "Value (cm)") + );
	$data = preg_replace("/^\\s+/m", "", $data);
	*/
	
	$r = preg_match_all("/"
		. "<[\w]+[^>]*>([0-9\\/]+ [0-9:]+)<\/[^>]*>"
		. "\s*<[\w]+[^>]*>([^<]+)<\/[^>]*>"
		. "\s*<[\w]+[^>]*>([-]?\d+)<\/[^>]*>"
		. "/is", $data, $matches);
	
	if($r !== false) {
		$result = array();
		
		for($i = 0; $i < $r; $i ++) {
			$result[] = array(
				"t" => $matches[1][$i],
				"x" => ($matches[2][$i] == "maximum" ? "+" : "-"),
				"v" => $matches[3][$i],
			);
		}
		
		file_put_contents("tidedata.json", json_encode($result));
		file_put_contents("tidedata.nexttime", time() + rand(3600, 3600*2));
	}
}