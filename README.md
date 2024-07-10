# Todo backend app


## Things I Learned:

**Minimal APIs:**  
Minimal API's take in two arguments: a API route, and a handler function. The handler The handler function takes the JSON data as a paramter, but it could also be something from an api endpoint such as the ID. Additionally, the function handler can take in the Todo service as the last parameter

**Middleware:** 
Implemented my own middleware to log request information. Middleware executes before and after requests are completed. Middleware functions take in a context and next. The context contains the request information and we use next to continue to the next middleware like this: `await next(context)`.

**Endpoint Filters:**
Execute before a endpoint. Used for things like input validation. Also takes in context and next and which is used to call the next endpoint filter.

**Dependency Injection:**
I learned how to implement services using an interface. The interface was a todo service that adds, deletes and gets todos. My implementation used in memory data from `List<Todo>`, but I could swtich to an implementation using a database without affecting the rest of the code.

**Records:**
Concise way to implement a describe data

**LINQ:**  
Learned a bit of LINQ, which works like lambda functions in other programming languages.

