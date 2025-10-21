import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HuespedesListComponent } from './huespedes-list.component';
import { RouterModule } from '@angular/router';
import { OrderByNombrePipe } from './order-by-nombre.pipe';

@NgModule({
  declarations: [
    HuespedesListComponent,
    OrderByNombrePipe
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', component: HuespedesListComponent }
    ])
  ]
})
export class HuespedesListModule { }