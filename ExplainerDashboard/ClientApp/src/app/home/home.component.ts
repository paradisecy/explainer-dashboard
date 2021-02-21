import { AfterViewInit, Component, ViewChild } from '@angular/core';
import * as AspNetData from "devextreme-aspnet-data-nojquery";
import MojsCurveEditor from '@mojs/curve-editor';
import { SolutionService } from '../services/solution-service';
import { DxChartComponent, DxDataGridComponent, DxPivotGridComponent } from 'devextreme-angular';
import CustomStore from 'devextreme/data/custom_store';
declare var mojs: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent implements AfterViewInit {
  demandColumns
  columns
  dataSource:CustomStore
  demandDataSource
  kb
  coach
  days
  currentDay = 1
  companies
  pivotGridDataSource
  @ViewChild('flockGrid', { static: false }) flockGrid : DxDataGridComponent;
  @ViewChild('demandGrid', { static: false }) demandGrid: DxDataGridComponent;

  @ViewChild('pivotgrid', { static: false }) pivotGrid: DxPivotGridComponent;
  @ViewChild('chart', { static: false }) chart: DxChartComponent;
  constructor(private solutionService: SolutionService) {
    this.companies = [{ "ID": 1, "name": "Flock Data" }, { "ID": 2, "name": "Pivot" }, { "ID": 3, "name": "Demand" }]
    this.days = [...Array(31).keys()];
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
        dataField: "fcr", caption: "Fcr"
      },
      {
        dataField: "yield", caption: "Yield"
      },
      {
        dataField: "mortalityRate", caption: "Mortality"
      },
      {
        dataField: "liveChickQuantity", caption: "Qty"
      },
      {
        dataField: "averageWeight", caption: "Avg Weight"
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



   

  }
  dayValueChange(ev) {

    this.currentDay = ev.value;
    this.demandGrid.instance.getDataSource().reload();
    this.flockGrid.instance.getDataSource().reload();

  }


  ngAfterViewInit() {


    setTimeout(() => {

      this.pivotGrid.instance.bindChart(this.chart.instance, {
        dataFieldsDisplayMode: "splitPanes",
        alternateDataFields: false
      });
      var dataSource = this.pivotGrid.instance.getDataSource();
      dataSource.expandHeaderItem('row', ['North America']);
      dataSource.expandHeaderItem('column', [2013]);


      this.pivotGridDataSource = {
        fields: [{
          caption: "day",
          width: 120,
          dataField: "day",
          area: "row",
          sortBySummaryField: "Total"
        },

          //  {
          //  caption: "City",
          //  dataField: "city",
          //  width: 150,
          //  area: "row"
          //}, {
          //  dataField: "date",
          //  dataType: "date",
          //  area: "column"
          //}, {
          //  groupName: "date",
          //  groupInterval: "month",
          //  visible: false
          //}, {
          //  caption: "Total",
          //  dataField: "amount",
          //  dataType: "number",
          //  summaryType: "sum",
          //  format: "currency",
          //  area: "data"
          //}
        ],
        store: this.flockGrid.instance.getDataSource().items()
      }
    }, 1000);

  }
}
