import { AfterViewInit, Component, ViewChild } from '@angular/core';
import * as AspNetData from "devextreme-aspnet-data-nojquery";
import MojsCurveEditor from '@mojs/curve-editor';
import { SolutionService } from '../services/solution-service';
import { DxChartComponent, DxDataGridComponent, DxPivotGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
import notify from 'devextreme/ui/notify';
declare var mojs: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent implements AfterViewInit {
  costDataSource
  costColumns
  bellCurveData;
  listSelectedItem = null;
  demandColumns
  decisionColumns
  columns
  dataSource: CustomStore
  demandDataSource
  decisionDataSource
  kb
  coach
  days
  currentDay = 1
  companies
  pivotGridDataSource
  selectedItems
  lowAverage = 10
  highAverage = 50
  chart_visualRange = [1, 5];
  @ViewChild('flockGrid', { static: false }) flockGrid: DxDataGridComponent;
  @ViewChild('demandGrid', { static: false }) demandGrid: DxDataGridComponent;
  @ViewChild('decisionGrid', { static: false }) decisionGrid: DxDataGridComponent;

  @ViewChild('pivotGrid', { static: false }) pivotGrid: DxPivotGridComponent;
  @ViewChild('chart', { static: false }) chart: DxChartComponent;
  constructor(private solutionService: SolutionService) {
    this.companies = [
      { "ID": 1, "name": "Flock Data" },
      { "ID": 2, "name": "Pivot" },
      { "ID": 3, "name": "Demand" },
      { "ID": 4, "name": "Decision" },
      { "ID": 5, "name": "Cost" }
    ];

    this.days = [...Array(50).keys()];
    this.days.shift()

    this.columns = [
      {
        dataField: "day", caption: "Day"
      },
      {
        dataField: "growingDay", caption: "Grow Day"
      },
      {
        dataField: "plantName", caption: "Plant"
      },
      {
        dataField: "group", caption: "Group"
      },
      {
        dataField: "coefficientVariation", caption: "CV"
      },
      {
        dataField: "avgCoefficientVariation", caption: "CV Avg"
      },
      {
        dataField: "varationClass", caption: "CV"
      },
      {
        dataField: "fcr", caption: "Fcr"
      },
      {
        dataField: "yield", caption: "Yield"
      },
      {
        dataField: "mortalityRate", caption: "Mortality"
      },
      {
        dataField: "avgMortalityRate", caption: "Avg Mortality"
      },
      {
        dataField: "liveChickQuantity", caption: "Qty"
      },
      {
        dataField: "averageWeight", caption: "Avg Weight"
      },
      {
        dataField: "feedConsumption", caption: "Feed Consumption"
      },
      {
        dataField: "totalFeedConsumption", caption: "Total Feed Consumption"
      },
      {
        dataField: "targetWeight", caption: "Target Weight"
      },
      {
        dataField: "cost", caption: "Cost"
      },
      {
        dataField: "costPerBird", caption: "Cost Per Bird"
      },
      {
        dataField: "totalCostPerBird", caption: "Total Cost Per Bird"
      },
      {
        dataField: "totalCost", caption: "Total Cost"
      },
      {
        dataField: "distance", caption: "Distance"
      },
      {
        dataField: "classAPercentage", caption: "A %"
      },
      {
        dataField: "classBPercentage", caption: "B %",
      },
    ];



    this.demandColumns = [
      {
        dataField: "day", caption: "Day"
      },
      {
        dataField: "quantity", caption: "Qty"
      },
      {
        dataField: "class", caption: "Class"
      },
      {
        dataField: "fromWeight", caption: "From Weight"
      },
      {
        dataField: "toWeight", caption: "To Weight"
      },
    ];


    this.decisionColumns = [
      {
        dataField: "day", caption: "Day"
      },
      {
        dataField: "plant", caption: "Plant"
      },
      {
        dataField: "quantity", caption: "Qty"
      },
    ];


    this.costColumns = [
      {
        dataField: "dayFrom", caption: "Day From"
      },
      {
        dataField: "dayTo", caption: "Day To"
      },
      {
        dataField: "costPerKg", caption: "Cost Per Kg"
      },
    ];



    this.dataSource = AspNetData.createStore({
      key: "id",
      loadUrl: "/data/get-flocks",
      insertUrl: "/data/create-flock",
      updateUrl: "/data/update-flock",
      deleteUrl: "/data/delete-flock",

      onBeforeSend: (method, ajaxOptions) => {
        if (method === "load") {
          ajaxOptions.url = ajaxOptions.url + `?day=${this.currentDay}`;
        }

      },

      onAjaxError(response) {
        console.log(response)

      },
    });

    this.demandDataSource = AspNetData.createStore({
      key: "id",
      loadUrl: "/demand/get-demand",
      insertUrl: "/demand/create-demand",
      updateUrl: "/demand/update-demand",
      deleteUrl: "/demand/delete-demand",


      onBeforeSend: (method, ajaxOptions) => {
        if (method === "load") {
          ajaxOptions.url = ajaxOptions.url + `?day=${this.currentDay}`;
        }
      },

      onAjaxError(response) {
        console.log(response)

      },
    });

    this.costDataSource = AspNetData.createStore({
      key: "id",
      loadUrl: "/cost/get-costs",
      insertUrl: "/cost/create-cost",
      updateUrl: "/cost/update-cost",
      deleteUrl: "/cost/delete-cost",


      onBeforeSend: (method, ajaxOptions) => {
        if (method === "load") {
          ajaxOptions.url = ajaxOptions.url + `?day=${this.currentDay}`;
        }
      },

      onAjaxError(response) {
        console.log(response)

      },
    });



    this.decisionDataSource = AspNetData.createStore({
      key: "id",
      loadUrl: "/decision/get-decisions",
      insertUrl: "/decision/create-decision",
      updateUrl: "/decision/update-decision",
      deleteUrl: "/decision/delete-decision",


      onBeforeSend: (method, ajaxOptions) => {
        if (method === "load") {
          ajaxOptions.url = ajaxOptions.url + `?day=${this.currentDay}`;
        }
      },

      onAjaxError(response) {
        console.log(response)

      },
    });


    this.selectedItems = [this.companies[0]]

  }



  onCellPrepared(cellInfo) {
    if (cellInfo.rowType == "data" && cellInfo.column.dataField === 'averageWeight') {
      if (cellInfo.data.averageWeight < cellInfo.data.targetWeight) {
        cellInfo.cellElement.style.color ='red';
      }
    }
  }

  dayValueChange(ev) {

    this.currentDay = ev.value;
    this.decisionGrid.instance.getDataSource().reload();
    this.demandGrid.instance.getDataSource().reload();
    this.flockGrid.instance.getDataSource().reload();


    this.pivotInit()
    this.generateContext(null)

  }

  generateContext(ev) {
    this.solutionService
      .generateContext(this.currentDay)
      .subscribe((data:any) => {


        this.solutionService.currentSolution.coach = data.result;

      })
  }

  pivotInit() {
    this.solutionService.getFlock().subscribe(data => {
        this.pivotGridDataSource = {
          fields:
            [
              {
                caption: "Group",
                dataField: "group",
                width: 150,
                area: "row",
                expanded: true,
              },
              {
                caption: "Grow Day",
                dataField: "growingDay",
                width: 150,
                area: "row",
                expanded: true,
              },
              {
                dataField: "growingDay",
                width: 150,
                area: "filter",
                filterType: 'include',
                filterValues: [this.currentDay]
              },
       
              {
                caption: "Plant",
                dataField: "plantName",
                width: 150,
                area: "row",
                expanded: true,
              },
              {
                caption: "Plant",
                dataField: "plantName",
                width: 150,
                area: "filter",
                filterType: 'include',
                filterValues: this.solutionService.currentSolution.suggestedPlantsList
              },
              {
                caption: "Group",
                dataField: "group",
                width: 150,
                area: "column"
              },
              {
                caption: "CV",
                dataField: "coefficientVariation",
                summaryType: "avg",
                format: { type: 'fixedPoint', precision: 2 },
                area: "data"
              },
              {
                caption: "FCR",
                dataField: "fcr",
                summaryType: "avg",
                format: { type: 'fixedPoint', precision: 2 },
                area: "data"
              },
              {
                caption: "Mortality",
                dataField: "mortalityRate",
                summaryType: "avg",
                format: { type: 'fixedPoint', precision: 2 },
                area: "data"
              },
              {
                caption: "Weight",
                dataField: "averageWeight",
                summaryType: "avg",
                format: { type: 'fixedPoint', precision: 2 },
                area: "data"
              },
              {
                caption: "Qty",
                dataField: "liveChickQuantity",
                dataType: "number",
                summaryType: "sum",
                format: "decimal",
                area: "data"
              },

            ],
          store: data
      }
      })

  }

  customizeTooltip(args) {

    var l = args.seriesName.split("-")
    var plant = l[l.length -1].split("|")[0]



    return {
      html: `Plant ${plant} | ${args.valueText}`
    };
  }

  onInitialized(ev) {

    try {
      this.pivotGrid.instance.bindChart(this.chart.instance, {
        dataFieldsDisplayMode: "splitPanes",
        alternateDataFields: true,
        inverted: false,
        customizeSeries: function (seriesName, seriesOptions) {
          // Change series properties here
          return seriesOptions; // This line is optional
        },
       customizeChart: function (chartOptions) {
          // Change chart properties here
          return chartOptions; // This line is optional
        }
      });
      this.chart.instance.element().scrollIntoView();
    } catch (e) {

    }

  }


  onListSelectionChanged(ev) {
    this.listSelectedItem = ev.addedItems[0]
    var p = this.flockGrid.instance.getDataSource().items().filter(f => f.plantName === this.listSelectedItem)[0]
    var std = (p.averageWeight * p.coefficientVariation) / 100
    var mean = p.averageWeight;

    this.solutionService.getBellCurve(mean, std).subscribe((s: any) => {

      this.bellCurveData = s.points
      this.chart_visualRange = [s.min,s.max]
    })
  }


  ngAfterViewInit() {
    this.pivotInit()
  }

  ngOnInit() {
    this.solutionService.reloadPivot.subscribe((result: boolean) => {
      this.pivotInit()

      this.selectedItems = [this.companies[1]]

    });
  }
}
