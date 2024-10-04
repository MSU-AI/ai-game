import pygame
import sys

# Initialize Pygame
pygame.init()

# Set up the game window
WIDTH = 800
HEIGHT = 400
SCREEN = pygame.display.set_mode((WIDTH, HEIGHT))
pygame.display.set_caption("Pygame Fighting Game")

# Colors
WHITE = (255, 255, 255)
RED = (255, 0, 0)
BLUE = (0, 0, 255)
GREEN = (0, 255, 0)

# Player properties
PLAYER_WIDTH = 50
PLAYER_HEIGHT = 100
PLAYER_SPEED = 5
JUMP_STRENGTH = 15
GRAVITY = 0.8

class Player:
    def __init__(self, x, y, color):
        self.rect = pygame.Rect(x, y, PLAYER_WIDTH, PLAYER_HEIGHT)
        self.color = color
        self.speed = PLAYER_SPEED
        self.direction = 0
        self.y_velocity = 0
        self.is_jumping = False
        self.health = 100
        self.attacking = False
        self.attack_cooldown = 0

    def move(self):
        self.rect.x += self.direction * self.speed
        # Keep player within screen bounds
        self.rect.x = max(0, min(self.rect.x, WIDTH - PLAYER_WIDTH))

        # Apply gravity
        self.y_velocity += GRAVITY
        self.rect.y += self.y_velocity

        # Check for ground collision
        if self.rect.bottom > HEIGHT:
            self.rect.bottom = HEIGHT
            self.y_velocity = 0
            self.is_jumping = False

    def jump(self):
        if not self.is_jumping:
            self.y_velocity = -JUMP_STRENGTH
            self.is_jumping = True

    def attack(self):
        if self.attack_cooldown == 0:
            self.attacking = True
            self.attack_cooldown = 30  # Set cooldown to 30 frames (0.5 seconds at 60 FPS)

    def update(self):
        self.move()
        if self.attack_cooldown > 0:
            self.attack_cooldown -= 1
        else:
            self.attacking = False

    def draw(self):
        pygame.draw.rect(SCREEN, self.color, self.rect)
        if self.attacking:
            attack_rect = pygame.Rect(self.rect.right, self.rect.y, 20, self.rect.height)
            if self.color == RED:  # If it's player 1, attack to the right
                attack_rect.left = self.rect.right
            else:  # If it's player 2, attack to the left
                attack_rect.right = self.rect.left
            pygame.draw.rect(SCREEN, GREEN, attack_rect)
        
        # Draw health bar
        health_rect = pygame.Rect(self.rect.x, self.rect.y - 10, self.rect.width * (self.health / 100), 5)
        pygame.draw.rect(SCREEN, GREEN, health_rect)

# Create players
player1 = Player(100, HEIGHT - PLAYER_HEIGHT, RED)
player2 = Player(WIDTH - 100 - PLAYER_WIDTH, HEIGHT - PLAYER_HEIGHT, BLUE)

# Game loop
clock = pygame.time.Clock()
running = True

while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.KEYDOWN:
            if event.key == pygame.K_a:
                player1.direction = -1
            elif event.key == pygame.K_d:
                player1.direction = 1
            elif event.key == pygame.K_w:
                player1.jump()
            elif event.key == pygame.K_SPACE:
                player1.attack()
            elif event.key == pygame.K_LEFT:
                player2.direction = -1
            elif event.key == pygame.K_RIGHT:
                player2.direction = 1
            elif event.key == pygame.K_UP:
                player2.jump()
            elif event.key == pygame.K_RETURN:
                player2.attack()
        elif event.type == pygame.KEYUP:
            if event.key in (pygame.K_a, pygame.K_d):
                player1.direction = 0
            elif event.key in (pygame.K_LEFT, pygame.K_RIGHT):
                player2.direction = 0

    # Update game state
    player1.update()
    player2.update()

    # Check for attacks
    if player1.attacking and player1.rect.right >= player2.rect.left and player1.rect.left < player2.rect.right:
        player2.health -= 1
    if player2.attacking and player2.rect.left <= player1.rect.right and player2.rect.right > player1.rect.left:
        player1.health -= 1

    # Draw everything
    SCREEN.fill(WHITE)
    player1.draw()
    player2.draw()
    pygame.display.flip()

    # Cap the frame rate
    clock.tick(60)

pygame.quit()
sys.exit()