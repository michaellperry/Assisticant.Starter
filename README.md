Assisticant.Starter
===================

Pared down version of Assisticant that can be used with Xamarin Starter Edition.

Data bind iOS and Android views to view models using dependency tracking. Create ViewControllers and Activities
just like you are used to, write simple view model classes, and set up two-way data binding.

## Observable

Declare all of your model fields using the Observable<T> class. Initialize the field, optionally initialize the value
by passing it into the constructor. Write properties to access the observable fields.

```c#
using Assisticant.Fields;

public class MyModel
{
    private Observable<string> _firstName = new Observable<string>("John");
    private Observable<string> _lastName = new Observable<string>("Doe");
    private Observable<string> _description = new Observable<string>();
    
    public string FirstName
    {
        get { return _firstName.Value; }
        set { _firstName.Value = value; }
    }
    
    public string LastName
    {
        get { return _lastName.Value; }
        set { _lastName.Value = value; }
    }
    
    public string Description
    {
        get { return _description.Value; }
        set { _description.Value = value; }
    }
}
```

## View Models

Write a class that holds a read only reference to the model. Initialize this reference in the constructor. Create properties
that get and set properties of the model. You can do any kind of calculation that you want. This class has no base class.

```c#
public class MyViewModel
{
    private readonly MyModel _model;
    
    public MyViewModel(MyModel model)
    {
        _model = model;
    }
    
    public string Name
    {
        get { return _model.FirstName + " " + _model.LastName; }
    }
    
    public string Description
    {
        get { return _model.Description; }
        set { _model.Description = value; }
    }
}
```

## BindingManager

In iOS, create a ViewController. In Android, create an Activity. Create a private field of type BindingManager. Also create
a field to hold your view model.

```c#
using Assisticant.Binding;

public class MyViewController : ViewController
{
    private BindingManager _bindings = new BindingManager();
    private MyViewModel _viewModel = new MyViewModel(new MyModel());
}
```

```c#
using Assisticant.Binding;

public class MyActivity : Activity
{
    private BindingManager _bindings = new BindingManager();
    private MyViewModel _viewModel = new MyViewModel(new MyModel());
}
```

## Initialize

In iOS, call the BindingManager's Initialize method in ViewDidLoad. Pass in the ViewController (this).

In Android, call the BindingManager's Initialize method in OnCreate. Pass in the Activity (this).

```c#
    protected override void ViewDidLoad()
    {
        base.ViewDidLoad();

        _bindings.Initialize(this);
    }
```

```c#
    protected override void OnCreate()
    {
        base.OnCreate();

        _bindings.Initialize(this);
    }
```

## Bind

In iOS, call the BindingManager's Bind methods in ViewWillAppear. Pass in the UI control to bind, a lambda expression
for output, and optionally a second lambda expression for input.

In Android, call the BindingManager's Bind methods in OnCreate, after the call to Initialize. Find the view to bind by
ID, and pass it into bind. Also pass in a lambda expression for output, and optionally a second lambda expression for input.

```c#
    protected override void ViewWillAppear()
    {
        base.ViewWillAppear();

        _bindings.BindText(nameLabel,
            () => _viewModel.Name);
        _bindings.BindText(descriptionEdit,
            () => _viewModel.Description,
            s => _viewModel.Description = s);
    }
```

```c#
    protected override void OnCreate()
    {
        base.OnCreate();

        _bindings.Initialize(this);
        
        _bindings.BindText(FindViewById<TextView>(Resources.Id.NameLabel),
            () => _viewModel.Name);
        _bindings.BindText(FindViewById<EditText>(Resources.Id.DescriptionEdit),
            () => _viewModel.Description,
            s => _viewModel.Description = s);
    }
```

## Unbind

In iOS, call the BindingManager's Unbind method in ViewDidDisappear. In Android, call it in OnDestroy.

```c#
    protected override void ViewDidDisappear()
    {
        _bindings.Unbind();

        base.ViewDidDisappear();
    }
```

```c#
    protected override void OnDestroy()
    {
        _bindings.Unbind();

        base.OnDestroy();
    }
```
