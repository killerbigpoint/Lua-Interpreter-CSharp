var test = 1;
var number = 1.5f;

function OnUpdate()
{
	var carsten = "omegalul";
	var magisk = 1337;
	
	Debug.Log(carsten);
	
	carsten = "sejereje";
	
	Debug.Log(carsten);
	
	Debug.Log("Hello World");
	Debug.Log("Hello", "World");
	
	if(1 == 2)
	{
		Debug.Log("Test 1");
	}
	else
	{
		Debug.Log("Test 2");
	}
	
	if(4 == 4)
	{
		Debug.Log("Test 3");
		
		if(5 == 2)
		{
			Debug.Log("Test 4");
		}
		else
		{
			Debug.Log("Test 5");
		}
	}
	else
	{
		Debug.Log("Test 6");
	}
}

function OnFixedUpdate()
{

}