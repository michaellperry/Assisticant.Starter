Assisticant.Starter
===================

Pared down version of Assisticant that can be used with Xamarin Starter Edition.

Data bind iOS and Android views to view models using dependency tracking. Create ViewControllers and Activities
just like you are used to, write simple view model classes, and set up two-way data binding.

```
    PM> Install-Package Assisticant.Starter
```

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

## ObservableList

In your model, define lists using the ObservableList<T> class. This has the same contract
as a List<T>, but it participates in data binding. Access the list through methods and
properties.

```c#
public class AddressBook
{
    private ObservableList<Person> _people = new ObservableList<Person>();

    public IEnumerable<Person> People
    {
        get { return _people; }
    }

    public Person NewPerson()
    {
        var person = new Person();
        _people.Add(person);
        return person;
    }
}
```

## View model collections

You could return a collection of model objects from the view model. But if you do, then
you will not have an opportunity to add view-specific properties to those objects. So I
recommend returning child view models instead.

Use Linq to project model objects into new view model objects.

```c#
public class AddressBookViewModel
{
    private readonly AddressBook _addressBook;

    public AddressBookViewModel(AddressBook addressBook)
    {
        _addressBook = addressBook;
    }

    public IEnumerable<PersonViewModel> People
    {
        get
        {
            return
                from person in _addressBook.People
                select new PersonViewModel(person);
        }
    }
}
```

## Child view model comparison

When you define child view models, it's a good idea to define Equals and GetHashCode. This
helps Assisticant keep the items in the view consistent with the child objects.

```c#
public class PersonViewModel
{
    private readonly Person _person;

    public PersonViewModel(Person person)
    {
        _person = person;            
    }

    public string Name
    {
        get { return _person.LastName + ", " + _person.FirstName; }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return _person == ((PersonViewModel)obj)._person;
    }

    public override int GetHashCode()
    {
        return _person.GetHashCode();
    }
}
```

## BindItems

To bind child view models to an Android ListView or an iOS UITableView, use the BindItems
method. In Android, pass in the identifier of the child layout, and a function that binds
each child.

```c#
    _bindings.BindItems(FindViewById<ListView>(Resource.Id.listPeople),
        () => _viewModel.People,
        Resource.Layout.Name,
        (view, person, bindings) =>
        {
            bindings.BindText(view.FindViewById<TextView>(Resource.Id.textName),
                () => person.Name);
        });
```

In iOS, pass in a function that binds each child to a UITableViewCell.

```c#
    _bindings.BindItems(listPeople,
        () => _viewModel.People,
        (cell, person, bindings) =>
        {
            bindings.BindText(cell.TextLabel,
                () => person.Name);
        });
```

The child bindings will be cleaned up when the item is removed from the list, or when
you call Unbind on the parent binding. There is no additional work for you to do.
