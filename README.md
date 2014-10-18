Assisticant.Starter
===================

Pared down version of Assisticant that fits inside of the Xamarin starter cap.

Data bind iOS and Android views to view models using dependency tracking. Create ViewControllers and Activities
just like you are used to, write simple view model classes, and set up two-way data binding.

## BindingManager

In iOS, create a ViewController. In Android, create an Activity. Create a private field of type BindingManager. Also create
a field to hold your view model.

```csharp
using Assisticant.Binding;

public class MyViewController : ViewController
{
    private BindingManager _bindings = new BindingManager();
    private MyViewModel _viewModel = new MyViewModel(new MyModel());
}
```

```csharp
using Assisticant.Binding;

public class MyActivity : Activity
{
    private BindingManager _bindings = new BindingManager();
    private MyViewModel _viewModel = new MyViewModel(new MyModel());
}
```

## Initialize

In iOS, call the BindingManager's Initialize method in ViewDidLoad. Pass in the ViewController (this).

In Android, call the BindingManager's Initialize method in OnCreated. Pass in the Activity (this).

```csharp
    protected override void ViewDidLoad()
    {
        _bindings.Initialize(this);
    }
```

```csharp
    protected override void OnCreated()
    {
        _bindings.Initialize(this);
    }
```

## Bind

In iOS, call the BindingManager's Bind methods in ViewWillAppear. Pass in the UI control to bind, a lambda expression
for output, and optionally a second lambda expression for input.

In Android, call the BindingManager's Bind methods in OnCreated, after the call to Initialize. Find the view to bind by
ID, and pass it into bind. Also pass in a lambda expression for output, and optionally a second lambda expression for input.

```csharp
    protected override void ViewWillAppear()
    {
        _bindings.BindText(nameLabel,
            () => _viewModel.Name);
        _bindings.BindText(descriptionEdit,
            () => _viewModel.Description,
            s => _viewModel.Description = s);
    }
```

```csharp
    protected override void OnCreated()
    {
        _bindings.Initialize(this);
        
        _bindings.BindText(FindViewById<TextView>(Resources.Id.NameLabel),
            () => _viewModel.Name);
        _bindings.BindText(FindViewById<EditText>(Resources.Id.DescriptionEdit),
            () => _viewModel.Description,
            s => _viewModel.Description = s);
    }
```

## Unbind

In iOS, call the BindingManager's Unbind method in ViewDidDisappear. In Android, call it in OnDestroyed.

```csharp
    protected override void ViewDidDisappear()
    {
        _bindings.Unbind();
    }
```

```csharp
    protected override void OnDestroyed()
    {
        _bindings.Unbind();
    }
```
