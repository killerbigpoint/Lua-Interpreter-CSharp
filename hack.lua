int test = 1;
float number = 1.5f;

function OnUpdate()
{
	console.boolean(true);
	console.boolean(false);
	
	console.log("Hello World");
	console.log("Hello", "World");
	
	if(1 == 2)
	{
		console.log("Israel");
	}
	else
	{
		console.log("Hebrew");
	}
	
	if(4 == 4)
	{
		console.log("Denmark");
		
		if(5 == 2)
		{
			console.log("Germany");
		}
		else
		{
			console.log("German");
		}
	}
	else
	{
		console.log("Danish");
	}
}

function OnFixedUpdate()
{

}