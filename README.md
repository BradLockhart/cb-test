## Run

Requires Docker.

```bash
docker compose up --build
```

Open:

- Application: http://localhost:8080
- Swagger: http://localhost:8080/swagger

## Test

```bash
dotnet test Interview.sln
```


## UPDATE - AI usage and engineering rationale

I've updated the project files within this commit, with what is basically me narrating through the high-level components within the code with brief line by line thoughts and understanding of what my solution is doing throughout the Program.cs, Controller, Service, and Repository within the .NET sln, and the App.tsx within the React app.



I did complete this task AI-assisted, but it is not "vibe-coded", every implementation in the .NET solution is fully understood, reviewed, and decided by me as the engineer.

I have significantly implemented parts of the code manually, and significantly changed AI-generated code, AI was used for implementation efficiency, and knowledge consolidation in the same way I would have used google/stackoverflow pre-AI, but it did not replace my engineering judgement or ownership of the implementations.

On the React app, I "relied" on AI much heavier than I did on the API, largely due to the time constraints, I do have a working understanding of React having used it in complex implementations for multiple years as a sort of "secondary" responsibility in carrying out frontend tasks as a .NET dev.

Without AI, it just would have taken longer and much more documentation referral, but there is nothing within the solution which has been implemented without my working understanding as a software engineer once submitted.

Some things probably wouldn't have made it into the solution without AI, they might have been the likes of the reloadKey in the React app, a simple clever use of a useEffect dependency to trigger a reload, not my own idea, total credit to AI, something quite granular that I probably would have skipped - 
along with the AbortController signal / cancellation token.



My thoughts on reviewing the requirements of this task were to ensure that I would cover the fundamental parts of what would be necessary...

Pagination was the first thing which came to mind, Products could be in the thousands+, we'd never need a UI intended for human-use to load anywhere close to that, so it would make perfect sense to implement a paginated solution for this.

Pagination can be achieved in various ways and some are more suitable than others depending on use case, this extremely simple implementation of offset pagination wouldn't be ideal for massive quantities of data because the deeper the page - the more rows the database has to work through / skip, and if the data is frequently changing then that can cause inconsistency (being wrongly skipped / returned twice, etc).

I'm not massively familiar with cursor based pagination from memory and I didn't want to study it within the time constraints or AI-implement it without full understanding, which without that - I'm not even sure whether or not cursor based pagination is the right tool for this type of solution.



The next thing I thought of wanting to demonstrate an understanding of was Clean Architecture, I enjoy design principles, architectural patterns, and established clever ways of building software in general.

Clean Architecture is basically about separating responsibilities into layers and keeping the dependencies flowing inwards, the Domain contains the core entities, Application contains the use cases and defines the interfaces it needs, Infrastructure implements things like data access, and the API handles HTTP and wires everything together.

It decouples the framework and persistence from core application logic and makes things easier to test and extend, but as with all software development principles - needs for tradeoffs will always arise and trying to be too pure can result in more issues than you are trying to avoid in the first place.



I've also tried to demonstrate other arbitrary principles throughout, such as picking up the habit of creating sealed classes by default, I remember when JetBrains ReSharper started doing things like this before the days of AI-assistance, it's just something that stuck with me and made perfect sense because it was contrary to most code I'd see in general, it makes sense to need to be explicit in when to open something, and leaving it closed where possible.



