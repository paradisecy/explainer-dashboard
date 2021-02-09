import { AfterViewInit, Component, ViewChild } from '@angular/core';
import * as AspNetData from "devextreme-aspnet-data-nojquery";
import MojsCurveEditor from '@mojs/curve-editor';
import { SolutionService } from '../services/solution-service';
import { DxDataGridComponent } from 'devextreme-angular';
declare var mojs: any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent implements AfterViewInit {
  demandColumns
  columns
  dataSource
  demandDataSource
  kb
  coach
  days
  currentDay = 1
  @ViewChild('flockGrid', { static: false }) flockGrid : DxDataGridComponent;
  @ViewChild('demandGrid', { static: false }) demandGrid: DxDataGridComponent;
  constructor(private solutionService: SolutionService) {

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

    //const curveEditor = new MojsCurveEditor({
    //  // name of the curve editor
    //  name: 'bounce curve',

    //  // if should preserve state on page reloads
    //  isSaveState: false,

    //  // start path - will be loaded on initialization of the curve,
    //  // e.g. before any user modifications were made. Path of 'M0, 100 L100, 0' is set by default.
    //  startPath: 'M0, 100 L100, 0',

    //  easing:"M0, 100 C0, 100 30, 25 30, 25 C30, 25 60, 70 60, 70 C60, 70 100, 0 100, 0",

    //  // callback on path change, accepts path string
    //  onChange: function (path) {
    //    console.log(path)
    //  },

    //  // if should hide when minimized - useful when you try to embed
    //  isHiddenOnMin: false
    //});


  }
}
