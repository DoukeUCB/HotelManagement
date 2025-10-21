import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HabitacionesListComponent } from './habitaciones-list.component';
import { RouterModule } from '@angular/router';
import { OrderByNumeroPipe } from './order-by-numero.pipe';

@NgModule({
  declarations: [
    HabitacionesListComponent,
    OrderByNumeroPipe
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', component: HabitacionesListComponent }
    ])
  ]
})
export class HabitacionesListModule { }