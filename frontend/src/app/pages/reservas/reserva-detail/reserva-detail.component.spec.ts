import { ComponentFixture, TestBed } from '@angular/core/testing';

import ReservaDetailComponent from './reserva-detail.component';

describe('ReservaDetailComponent', () => {
  let component: ReservaDetailComponent;
  let fixture: ComponentFixture<ReservaDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReservaDetailComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ReservaDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
