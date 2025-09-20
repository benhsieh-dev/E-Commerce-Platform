import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatDividerModule,
    MatIconModule
  ],
  template: `
    <footer class="footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-section">
            <h3>AirBnB Platform</h3>
            <p>Discover amazing places to stay around the world.</p>
            <div class="social-links">
              <a href="#" aria-label="Facebook">
                <mat-icon>facebook</mat-icon>
              </a>
              <a href="#" aria-label="Twitter">
                <mat-icon>twitter</mat-icon>
              </a>
              <a href="#" aria-label="Instagram">
                <mat-icon>instagram</mat-icon>
              </a>
            </div>
          </div>

          <div class="footer-section">
            <h4>Support</h4>
            <ul>
              <li><a href="#" routerLink="/help">Help Center</a></li>
              <li><a href="#" routerLink="/safety">Safety Information</a></li>
              <li><a href="#" routerLink="/cancellation">Cancellation Options</a></li>
              <li><a href="#" routerLink="/contact">Contact Us</a></li>
            </ul>
          </div>

          <div class="footer-section">
            <h4>Community</h4>
            <ul>
              <li><a href="#" routerLink="/blog">Blog</a></li>
              <li><a href="#" routerLink="/events">Events</a></li>
              <li><a href="#" routerLink="/forum">Community Forum</a></li>
              <li><a href="#" routerLink="/newsroom">Newsroom</a></li>
            </ul>
          </div>

          <div class="footer-section">
            <h4>Hosting</h4>
            <ul>
              <li><a href="#" routerLink="/host">Become a Host</a></li>
              <li><a href="#" routerLink="/host-resources">Host Resources</a></li>
              <li><a href="#" routerLink="/host-community">Host Community</a></li>
              <li><a href="#" routerLink="/responsible-hosting">Responsible Hosting</a></li>
            </ul>
          </div>

          <div class="footer-section">
            <h4>Company</h4>
            <ul>
              <li><a href="#" routerLink="/about">About</a></li>
              <li><a href="#" routerLink="/careers">Careers</a></li>
              <li><a href="#" routerLink="/press">Press</a></li>
              <li><a href="#" routerLink="/investors">Investors</a></li>
            </ul>
          </div>
        </div>

        <mat-divider></mat-divider>

        <div class="footer-bottom">
          <div class="footer-legal">
            <span>&copy; 2024 AirBnB Platform. All rights reserved.</span>
            <div class="legal-links">
              <a href="#" routerLink="/privacy">Privacy Policy</a>
              <a href="#" routerLink="/terms">Terms of Service</a>
              <a href="#" routerLink="/sitemap">Sitemap</a>
            </div>
          </div>
          
          <div class="footer-info">
            <span>Version 1.0.0</span>
          </div>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background-color: #f7f7f7;
      padding: 40px 0 20px;
      margin-top: 60px;
      border-top: 1px solid #e0e0e0;
    }

    .footer-content {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1fr 1fr;
      gap: 40px;
      margin-bottom: 32px;
    }

    .footer-section h3 {
      color: #ff5a5f;
      font-size: 20px;
      font-weight: 700;
      margin-bottom: 16px;
    }

    .footer-section h4 {
      color: #484848;
      font-size: 16px;
      font-weight: 600;
      margin-bottom: 16px;
    }

    .footer-section p {
      color: #767676;
      margin-bottom: 20px;
      line-height: 1.5;
    }

    .footer-section ul {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .footer-section li {
      margin-bottom: 8px;
    }

    .footer-section a {
      color: #767676;
      text-decoration: none;
      transition: color 0.3s ease;

      &:hover {
        color: #ff5a5f;
      }
    }

    .social-links {
      display: flex;
      gap: 16px;
    }

    .social-links a {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      background-color: #e0e0e0;
      border-radius: 50%;
      transition: background-color 0.3s ease;

      &:hover {
        background-color: #ff5a5f;
        
        mat-icon {
          color: white;
        }
      }
    }

    .social-links mat-icon {
      color: #767676;
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .footer-bottom {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 20px;
    }

    .footer-legal {
      display: flex;
      align-items: center;
      gap: 24px;
    }

    .footer-legal span {
      color: #767676;
      font-size: 14px;
    }

    .legal-links {
      display: flex;
      gap: 20px;
    }

    .legal-links a {
      color: #767676;
      text-decoration: none;
      font-size: 14px;
      transition: color 0.3s ease;

      &:hover {
        color: #ff5a5f;
      }
    }

    .footer-info span {
      color: #b0b0b0;
      font-size: 12px;
    }

    @media (max-width: 1024px) {
      .footer-content {
        grid-template-columns: 1fr 1fr;
        gap: 32px;
      }
    }

    @media (max-width: 768px) {
      .footer-content {
        grid-template-columns: 1fr;
        gap: 24px;
      }

      .footer-bottom {
        flex-direction: column;
        gap: 16px;
        text-align: center;
      }

      .legal-links {
        flex-direction: column;
        gap: 8px;
      }
    }
  `]
})
export class FooterComponent {}