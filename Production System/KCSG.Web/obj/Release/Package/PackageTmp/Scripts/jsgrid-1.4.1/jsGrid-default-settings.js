jsGrid.setDefaults({
    width: 'auto', // Grid should be fulfilled the screen.
    loadIndication: false, // Disable the default indication icon of jsGrid (use the custom one)
    noDataContent: 'Cannot find relative record in the Database.',
    pageIndex: 1, // An integer value specifying current page index. Applied only when paging is true
    pageSize: 30, //An integer value specifying the amount of items on the page. Applied only when paging is true.
    pageButtonCount: 10, //An integer value specifying the maximum amount of page buttons to be displayed in the pager.
    pageNextText: 'Next',
    pagePrevText: 'Prev',
    pageFirstText: 'First',
    pageLastText: 'Last',
    paging: true,
    
})