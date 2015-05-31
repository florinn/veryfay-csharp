Veryfay (C#) [![build badge](https://travis-ci.org/florinn/veryfay-csharp.svg?branch=master)](https://travis-ci.org/florinn/veryfay-csharp)
===================

**Veryfay (C#)** is a port to C# of the [**Veryfay**](https://github.com/florinn/veryfay-scala) library in Scala.

----------


Features
-------------
* Define multiple authorization engines in the same application
* Define activities with or without a target class
* Specify allow or deny sets
* Associate roles to multiple activities through hierarchical activity containers
* Check authorization either by returning boolean or exception throwing


Installing
-------------
Add this **NuGet** dependency to your project: 

```
PM> Install-Package veryfay 
```


Usage
-------------

### Define authorization rules

This part consists of a few straightforward preparatory operations that culminate with the creation of an "authorization engine" to be used later to perform authorization verification.

##### Define any custom activities

An activity takes a type parameter describing the target for the activity (named `TTarget`), which may be any class defined in your application.

For activities with no target, you should specify **Nothing** as the `TTarget` type argument of the activity.

There are a few predefined activities: 
- *Create*
- *Read*
- *Update*
- *Patch*
- *Delete*

You may define your own activities by inheriting from `Activity<TTarget>`:

```csharp
public sealed class SomeActivity<TTarget> : Activity<TTarget> { }
```

##### Define any container activities

Container activities help with associating multiple actions to the same role(s).  
Instead of repeating the same activities over and over again, a container activity may be defined holding a list of activities (including container activities).

There a couple predefined container activities:
- *CRUD* containing activities: *Create*, *Read*, *Update*, *Delete*
- *CRUDP* containing activities: *CRUD*, *Patch*

Define your own container activities like so:

```csharp
public sealed class SomeContainerActivity<TTarget> : Activity<TTarget>, Container
{
    private Activity<TTarget>[] activities =
        new Activity<TTarget>[] { new SomeActivity<TTarget>(), new OtherActivity<TTarget>(), new SomeOtherActivity<TTarget>() };

    public Activity[] Activities
    {
        get { return activities; }
    }
}
```

>**Note:** Container activities are used only for defining authorization rules, they are not used when verifying authorization rules

##### Define roles

You may define a role by inheriting from `Role<TPrincipal, TExtraInfo>`, where
- *TPrincipal* is the type of the principal class passed into the role definition 
- *TExtraInfo* is the type of any extra info that may get passed into the role definition

In `Contains` you can place any logic to determine if the input data belongs to that role.

```csharp
public sealed class SomeRole : Role<SomePrincipalClass, SomeClass>
{
    private static readonly Lazy<SomeRole> instance = new Lazy<SomeRole>(() => new SomeRole());

    public static SomeRole Instance { get { return instance.Value; } }

    private SomeRole() { }

    public bool Contains(SomePrincipalClass principal, SomeClass extraInfo = default(SomeClass))
    {
        // Some logic to determine if input belongs to the role
    }
}
```

##### Configure authorization rules 

You may use `Register`, `Allow`, `Deny` and `And` to associate any allow and deny roles with one or more activities in the context of an authorization engine:

```csharp
AuthorizationEngine ae = new AuthorizationEngine()
    .Register(new CRUDP<Nothing>())
        .Allow(Admin.Instance).Deny(Supervisor.Instance, Commiter.Instance).Deny(Contributor.Instance).And
    .Register(new CRUDP<SomeOtherClass>())
        .Allow(Admin.Instance).Allow(Supervisor.Instance).Allow(Reader.Instance).Allow(Contributor.Instance).And
    .Register(new Create<Nothing>())
        .Allow(Commiter.Instance).Deny(Contributor.Instance).And
    .Register(new Read<Nothing>())
        .Allow(Commiter.Instance).Deny(Contributor.Instance).Allow(Reviewer.Instance).And
    .Register(new Read<SomeClass>())
        .Allow(Supervisor.Instance, Commiter.Instance).And
    .Register(new Read<SomeClass>(), new Read<SomeOtherClass>())
        .Allow(Supervisor.Instance).Allow(Contributor.Instance).Deny(Reader.Instance).And
    .Register(new Read<SomeClass>())
        .Allow(Reader.Instance).And
    .Register(new Read<OtherSomeOtherClass>())
        .Allow(Reader.Instance).Deny(Commiter.Instance).Allow(Reviewer.Instance).And;
```

>**Notes:** 
- Roles specified in the same argument list of `Allow` or `Deny` are bound together by logical *AND*
- Roles specified in separate argument lists of `Allow` or `Deny` are bound together by logical *OR*


### Verify authorization rules

To verify the authorization rules you may call either: 

- `IsAllowing` which returns `IsAllowingResult` containing the result of the verification as a bool value and a string with information about the execution of the authorization rules

```csharp
var result = ae[new Read<SomeOtherClass>()].IsAllowing(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
```
 
- `Verify` which returns `string` in case of success, otherwise throwing `AuthorizationException` containing a string with information about the execution of the authorization rules

```csharp
ae[new Read<SomeOtherClass>()].Verify(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
```

>**Note:** During rules verification, role definitions are matched both by activity type and by the types of the arguments (for principal and optionally extra info) which are passed in to `IsAllowing` or `Verify`