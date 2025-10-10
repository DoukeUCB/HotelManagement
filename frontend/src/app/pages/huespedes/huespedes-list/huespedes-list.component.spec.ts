import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HuespedesListComponent } from './huespedes-list.component';

describe('HuespedesListComponent', () => {
  let component: HuespedesListComponent;
  let fixture: ComponentFixture<HuespedesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HuespedesListComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(HuespedesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
