# A Rebuilt Company PDF Merger

This was my excuse to get more familiar with WPF and the MVVM architecture.
The whole idea of the application is based around creating merged pdfs for
individual contracts or entities. In our company, the contract acts as the parent of the pdf.

- Date picker is used during the PDF merging for the output file.
- The contract combobox allows the user to select the which contract to display during PDF creation.
- To create a contract, right click the combobox and an empty contract will be created (Note: removing contract removes the currently selected contract NOT the highlighted one. I hope to change this at some point)
- The work directory field is used as a shortcut when adding new pdfs to be merged
- Once a contract is selected, the name and number will be displayed and can be altered
- Right clicking the file list will open a context menu to add or remove PDFs. 
- PDFs may also be removed from the list using the 'delete' key.
- An output filename must be used before being able to merge the PDFs.
- In the output filename field, the user may enter shortcuts to auto enter program properties: {D} for date, {C} for contract number, and {N} for contract name.
- After merging the PDFs, the status is shown above the 'Output Filename' label.

##Concepts Learned and Used
- MVVM architecture: 
- Properties using OnPropertyChanged to notify UI of changes
- Data binding in XAML
- XAML to create a 'usable'(by me ;]) UI.
- Commands: CanExecute requirements, passing command parameter
- IValueConverter
- SQlite for local storage


