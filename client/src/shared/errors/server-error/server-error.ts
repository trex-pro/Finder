import {Component, inject} from '@angular/core';
import { Router } from '@angular/router';
import { ApiError } from '../../../types/error';

@Component({
  selector: 'app-server-error',
  imports: [],
  templateUrl: './server-error.html',
  styleUrl: './server-error.css',
})
export class ServerError {
  private router = inject(Router);
  protected error: ApiError;
  protected showDetails = false;

  constructor() {
    const navigation = this.router.currentNavigation();
    this.error = navigation?.extras?.state?.['error']
  }

  get topFrameOnly(): string {
    if (!this.error?.details) return 'No Details Available.';
    
    // Split the text by new lines and take the first item
    const lines = this.error.details.split('\n');
    return lines[0];
  }

  detailsToggle() {
    this.showDetails = !this.showDetails;
  }
}
