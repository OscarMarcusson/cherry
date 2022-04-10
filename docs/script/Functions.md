# Functions
A function is a block of code which can be called by other code or by UI events.

## Defining a function
Functions are defined by writing `def`, followed by the `type` and `name`. For example:
```d
// A function that takes no arguments and returns no value
def void do_something

// A function that takes no arguments and returns a value
def int return_something
```

If the function should have any arguments (variables sent into the code block) those are added by appending `:` and a comma separated list of `type name` declarations:
```d
// A function that takes 2 arguments and returns no value
def void do_something_else : int a, string b

// A function that takes 2 argument and returns a value
def int return_something_else : int a, string b
```

It is worth noting that in the case of void functions, that is to say functions that do not have a return value, the `void` part does not need to be written. 
The following code will for example be understood as a void function automatically:
```d
// Same as def void do_something
def do_something
```


### Function body
In order for the function to be compiled it needs to have a body, which is the actual code to be called. 
This is added on a new line, indented: 
```d
def do_something
	print("Hello World!")
```

If the function has a return type that is not void the function also needs a return statement:
```d
def int return_something
	return 5
```
