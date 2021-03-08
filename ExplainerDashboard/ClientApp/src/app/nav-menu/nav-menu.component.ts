import { Component, ViewChild } from '@angular/core';
import { SolutionService } from '../services/solution-service';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
})
export class NavMenuComponent {
  constructor(private solutionService: SolutionService) {
    this.getSolutions()

  }
  solution
  solutions
  newSolution
  isExpanded = false;
  popupVisible
  buttonOptions = {
    text: "Save",
    useSubmitBehavior: true
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  initializeData(ev) {

    this.solutionService
      .initData()
      .subscribe(s => {
        location.reload();
      })
  }

  createSolution(ev) {
    this.popupVisible = true;

  }

  processSolution() {
    this.solutionService
      .process(this.solutionService.currentSolution)
      .subscribe(s => {
        this.solutionService.currentSolution = s;
        this.solutionService.reloadPivot.next(true);
      })
  }

  removeSolution(ev) {
    this.solutionService
      .delete(this.solutionService.currentSolution)
      .subscribe((s: any) => {
        this.getSolutions()
        notify(s)
      })
  }

  saveSolutions(ev) {
    this.solutionService
      .save(this.solutionService.currentSolution)
      .subscribe((s: any) => {
        notify(s.message)
      })
  }

  getSolutions() {

    this.solutionService.get().subscribe(s => {
      this.solutions = s;

    })

  }

  onCreateFormSubmit(ev) {
    console.log(this.newSolution)
    this.solutionService.create(this.newSolution).subscribe((s: any) => {
      this.getSolutions()

      this.popupVisible = false;
      this.newSolution = {}
      notify(`Solution ${s.name} created successfully`)

    })

    ev.preventDefault();
  }

  handleValueChange(ev) {
    this.solutionService.get().subscribe(s => {
      this.solutions = s;
      this.solution = ev.value;
      this.solutionService.currentSolution = this.solutions.filter(f => f.id === ev.value)[0];
    })
  }
}
